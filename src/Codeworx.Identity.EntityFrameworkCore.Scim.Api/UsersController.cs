using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Error;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Binding;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Patch;
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
    [ScimError]
    public class UsersController : Controller
    {
        private readonly DbContext _db;
        private readonly IFilterParser _filterParser;
        private readonly IResourceMapper<User> _mapper;
        private readonly IPatchProcessor _patchProcessor;

        public UsersController(
            IContextWrapper contextWrapper,
            IFilterParser filterParser,
            IResourceMapper<User> mapper,
            IPatchProcessor patchProcessor)
        {
            _db = contextWrapper.Context;
            _filterParser = filterParser;
            _mapper = mapper;
            _patchProcessor = patchProcessor;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<ListResponse>> GetUsersAsync([FromQuery] int startIndex, [FromQuery] int count, [FromQuery] string? filter, [FromQuery] string? excludedAttributes, Guid providerId)
        {
            ConfigHelper.ValidateDefaultPagination(ref startIndex, ref count);

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

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
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
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        [ProducesResponseType(StatusCodes.Status409Conflict, Type = typeof(ErrorResource))]
        public async Task<ActionResult<UserResponse>> AddUserAsync([RequestResourceBinder] UserRequest user, Guid providerId)
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

        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> UpdateUsersAsync(Guid id, [RequestResourceBinder] UserRequest user, Guid providerId)
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

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
        public async Task<ActionResult<UserResponse>> PatchUserAsync(Guid id, [RequestResourceBinder] PatchRequest patch, Guid providerId)
        {
            var user = await GetUserAsync(id, null, providerId);

            var current = user?.Value;

            if (current == null)
            {
                return NotFound();
            }

            var userRequest = await _patchProcessor.ProcessAsync<UserResponse, UserRequest>(current, patch.Resource);

            return await UpdateUsersAsync(id, userRequest, providerId);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(ErrorResource))]
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
    }
}