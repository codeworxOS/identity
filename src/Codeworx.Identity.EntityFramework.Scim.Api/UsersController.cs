using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Binding;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/Users")]
    [Produces("application/scim+json", "application/json")]
    [Consumes("application/scim+json", "application/json")]
    [Authorize(Policy = ScimConstants.Policies.ScimInterop)]
    public class UsersController : Controller
    {
        private readonly DbContext _db;
        private readonly IFilterParser _filterParser;
        private readonly IResourceMapper<User> _mapper;
        private readonly IEnumerable<ISchemaExtension> _schemaExtensions;

        public UsersController(
            IContextWrapper contextWrapper,
            IFilterParser filterParser,
            IResourceMapper<User> mapper,
            IEnumerable<ISchemaExtension> schemaExtensions)
        {
            _db = contextWrapper.Context;
            _filterParser = filterParser;
            _mapper = mapper;
            _schemaExtensions = schemaExtensions;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListResponse>> GetUsersAsync([FromQuery] int startIndex, [FromQuery] int count, [FromQuery] string? filter, [FromQuery] string? excludedAttributes, Guid providerId)
        {
            ConfigHelper.ValidateDefaultPagination(ref startIndex, ref count);

            try
            {
                var query = from u in _db.Set<User>().AsNoTracking()
                            from p in u.Providers.Where(p => p.ProviderId == providerId)
                            select new ScimEntity<User> { Entity = u, ExternalId = p.ExternalIdentifier, ProviderId = p.ProviderId };

                BooleanFilterNode? filterNode = null;

                if (filter != null)
                {
                    filterNode = (BooleanFilterNode)_filterParser.Parse(filter);
                }

                var parameters = new QueryParameter(filterNode, excludedAttributes, providerId);

                var mapped = _mapper.GetResourceQuery(_db, query, parameters);

                var totalResults = await mapped.CountAsync();

                if (startIndex > 1)
                {
                    mapped = mapped.Skip(startIndex - 1);
                }

                if (count > 0)
                {
                    mapped = mapped.Take(count);
                }

                var users = await mapped.ToListAsync();

                var result = new List<UserResponse>();

                foreach (var item in users)
                {
                    UserResponse response = GenerateUserResponse(item.Values);

                    result.Add(response);
                }

                return new ListResponse(startIndex, totalResults, count, result);
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> GetUserAsync(Guid userId, [FromQuery] string? excludedAttributes, Guid providerId)
        {
            var query = from u in _db.Set<User>().AsNoTracking()
                        from p in u.Providers.Where(p => p.ProviderId == providerId)
                        where u.Id == userId
                        select new ScimEntity<User> { Entity = u, ExternalId = p.ExternalIdentifier, ProviderId = p.ProviderId };

            var parameters = new QueryParameter(null, excludedAttributes, providerId);

            var mapped = _mapper.GetResourceQuery(_db, query, parameters);

            var user = await mapped.FirstOrDefaultAsync().ConfigureAwait(false);

            if (user == null)
            {
                return NotFound();
            }

            UserResponse response = GenerateUserResponse(user.Values);

            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponse>> AddUserAsync([RequestResourceBinder] UserRequest user, Guid providerId)
        {
            try
            {
                using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    if (user.ExternalId == null)
                    {
                        return Conflict();
                    }

                    var existing = await _db.Set<AuthenticationProviderRightHolder>().Where(p => p.ProviderId == providerId && p.ExternalIdentifier == user.ExternalId)
                                        .AnyAsync();

                    if (existing)
                    {
                        return Conflict();
                    }

                    var item = new User
                    {
                        Id = Guid.NewGuid(),
                        Created = DateTime.UtcNow,
                        ForceChangePassword = true,
                        AuthenticationMode = Login.AuthenticationMode.Login,
                    };

                    var mapping = new AuthenticationProviderRightHolder
                    {
                        ExternalIdentifier = user.ExternalId,
                        ProviderId = providerId,
                        RightHolderId = item.Id,
                    };

                    _db.Add(item);
                    _db.Add(mapping);

                    await _mapper.ToDatabaseAsync(_db, item, user.Flatten(), providerId);

                    await _db.SaveChangesAsync();

                    await transaction.CommitAsync();

                    var response = await GetUserAsync(item.Id, null, providerId);
                    return CreatedAtAction("AddUser", response.Value);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> UpdateUsersAsync(Guid id, [RequestResourceBinder] UserRequest user, Guid providerId)
        {
            try
            {
                using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
                {
                    var item = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();
                    await _db.Set<AuthenticationProviderRightHolder>().Where(p => p.ProviderId == providerId && p.RightHolderId == id).LoadAsync();

                    if (item == null)
                    {
                        return NotFound();
                    }

                    await _mapper.ToDatabaseAsync(_db, item, user.Flatten(), providerId);

                    ////ApplyEntityChanges(user, item);

                    await _db.SaveChangesAsync();

                    await transaction.CommitAsync();

                    return await GetUserAsync(item.Id, null, providerId);
                }
            }
            catch (Exception)
            {
                return BadRequest();
            }
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> PatchUserAsync(Guid id, [RequestResourceBinder] PatchRequest patch, Guid providerId)
        {
            var options = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };

            var user = await GetUserAsync(id, null, providerId);

            var current = user?.Value;

            if (current == null)
            {
                return NotFound();
            }

            var json = JsonSerializer.SerializeToNode<UserResponse>(current, options)!.AsObject();

            foreach (var operation in patch.Operations)
            {
                if (operation.Path == null)
                {
                    if (operation.Op == PatchOp.Remove)
                    {
                        return BadRequest();
                    }

                    if (operation.Value is JsonElement node)
                    {
                        MergeProperties(JsonObject.Create(node)!, json);
                    }
                }
                else if (!operation.Path.Contains("["))
                {
                    var parsed = _filterParser.Parse(operation.Path);

                    if (operation.Value is JsonElement node)
                    {
                        var value = JsonValue.Create(node)!;

                        if (parsed is PathFilterNode path)
                        {
                            SetPropertyValue(path, json, value);
                        }
                    }
                }
                else
                {
                    if (operation.Op == PatchOp.Remove && operation.Value != null)
                    {
                        return BadRequest();
                    }
                    else if (operation.Op == PatchOp.Replace)
                    {
                        var parsed = _filterParser.Parse(operation.Path);
                        if (parsed is ArrayFilterNode array)
                        {
                            if (operation.Value is JsonElement node)
                            {
                                foreach (var row in array.GetItems(json))
                                {
                                    if (array.Member != null)
                                    {
                                        var value = JsonValue.Create(node)!;
                                        var memberParsed = _filterParser.Parse(array.Member);
                                        if (memberParsed is PathFilterNode path)
                                        {
                                            SetPropertyValue(path, row.AsObject(), value);
                                        }
                                    }
                                    else
                                    {
                                        MergeProperties(JsonObject.Create(node)!, row.AsObject());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            var options2 = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

            foreach (var extension in _schemaExtensions)
            {
                options2.Converters.Add(new ScimSchemaConverter(extension.Schema, extension.TargetType));
            }

            var userRequest = JsonSerializer.Deserialize<UserRequest>(json, options2)!;

            return await UpdateUsersAsync(id, userRequest, providerId);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUsersAsync(Guid id)
        {
            var item = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (item == null)
            {
                return NotFound();
            }

            var entry = _db.Remove(item);

            await _db.SaveChangesAsync();

            return NoContent();
        }

        private UserResponse GenerateUserResponse(IEnumerable<IScimResource> resources)
        {
            var info = resources.OfType<ScimResponseInfo>().FirstOrDefault();

            if (info == null)
            {
                throw new NotSupportedException("ScimResponseInfo must be mapped for entity!");
            }

            var userUrl = this.Url.ActionLink(controller: "Users", action: "GetUser", values: new { userId = info.Id })!;

            info.Location = userUrl;

            var user = resources.OfType<UserResource>().FirstOrDefault();

            if (user == null)
            {
                throw new NotSupportedException("At least one property of UserResource must be mapped!");
            }

            return new UserResponse(info, user, resources.OfType<ISchemaResource>().Except(new[] { user }).ToArray());
        }

        private void MergeProperties(JsonObject node, JsonObject target)
        {
            foreach (var item in node.ToList())
            {
                node.Remove(item.Key);

                var parsed = _filterParser.Parse(item.Key);

                if (parsed is PathFilterNode path)
                {
                    SetPropertyValue(path, target, item.Value!);
                }
            }
        }

        private void SetPropertyValue(PathFilterNode path, JsonObject target, JsonNode value)
        {
            JsonObject parent = target;

            if (path.Scheme != null)
            {
                if (!parent.TryGetPropertyValue(path.Scheme, out var schemeValue))
                {
                    schemeValue = new JsonObject();
                    parent.Add(path.Scheme, schemeValue);
                }

                parent = schemeValue!.AsObject();
            }

            var paths = path.Paths;

            for (int i = 0; i < paths.Length; i++)
            {
                if (i < paths.Length - 1)
                {
                    if (parent.TryGetPropertyValue(paths[i], out var next))
                    {
                        parent = next!.AsObject();
                    }
                    else
                    {
                        var child = new JsonObject();
                        parent.Add(paths[i], child);
                        parent = child;
                    }
                }
                else
                {
                    parent.Remove(paths[i]);
                    parent.Add(paths[i], value);
                }
            }
        }
    }
}