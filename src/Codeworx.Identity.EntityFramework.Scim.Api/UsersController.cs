using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Binding;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("{providerId}/scim/Users")]
    [AllowAnonymous]
    public class UsersController : Controller
    {
        private readonly DbContext _db;
        private readonly IResourceMapper<User> _mapper;

        public UsersController(IContextWrapper contextWrapper, IResourceMapper<User> mapper)
        {
            _db = contextWrapper.Context;
            _mapper = mapper;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse> GetUsersAsync([FromQuery] int startIndex, [FromQuery] int count, Guid providerId)
        {
            ConfigHelper.ValidateDefaultPagination(ref startIndex, ref count);

            var query = _db.Set<User>().AsNoTracking().OrderBy(c => c.Id).AsQueryable();
            var mapped = _mapper.GetResourceQuery(query);

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
        public async Task<ActionResult<UserResponse>> GetUserAsync(Guid userId, Guid providerId)
        {
            var query = _db.Set<User>().AsQueryable().Where(t => t.Id == userId);

            var mapped = _mapper.GetResourceQuery(query);

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
        [ProducesResponseType(StatusCodes.Status409Conflict)]
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

                await _mapper.ToDatabaseAsync(_db, item, user.Flatten());

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = await GetUserAsync(item.Id, providerId);
                return CreatedAtAction("AddUser", response.Value);
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> UpdateUsersAsync(Guid id, [RequestResourceBinder] UserRequest user, Guid providerId)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var item = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound();
                }

                await _mapper.ToDatabaseAsync(_db, item, user.Flatten());

                ////ApplyEntityChanges(user, item);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(await GetUserAsync(item.Id, providerId));
            }
        }

        ////[HttpPatch("{id}")]
        ////[ProducesResponseType(StatusCodes.Status200OK)]
        ////[ProducesResponseType(StatusCodes.Status200OK)]
        ////public async Task<ActionResult<UserResponse>> PatchUserAsync(Guid id, [FromBody] PatchOperation patch)
        ////{
        ////    var entity = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

        ////    if (entity == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    foreach (var operation in patch.Operations)
        ////    {
        ////        // todo apply operations
        ////    }

        ////    await _db.SaveChangesAsync();

        ////    return Ok(await GetUserAsync(entity.Id));
        ////}

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
            ////var info = new ScimResponseInfo(item.Id.ToString("N"), userUrl, item.Created, item.Created);

            var user = resources.OfType<UserResource>().FirstOrDefault();

            if (user == null)
            {
                throw new NotSupportedException("At least one property of UserResource must be mapped!");
            }

            return new UserResponse(info, user, resources.OfType<ISchemaResource>().Except(new[] { user }).ToArray());

            ////foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
            ////{
            ////    object data;
            ////    if (resourceType.Key == typeof(UserResource))
            ////    {
            ////        data = resource;
            ////    }
            ////    else
            ////    {
            ////        if (Activator.CreateInstance(resourceType.Key) is ISchemaResource d)
            ////        {
            ////            data = d;
            ////            list.Add(d);
            ////        }
            ////        else
            ////        {
            ////            throw new NotSupportedException();
            ////        }
            ////    }

            ////    foreach (var property in resourceType)
            ////    {
            ////        property.SetResourceValue(data, entityEntry.Property(property.EntityPropertyName).CurrentValue);
            ////    }
            ////}
        }

        ////private void ApplyEntityChanges(UserRequest user, User item)
        ////{
        ////    var entityEntry = _db.Entry(item);

        ////    foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
        ////    {
        ////        object data;
        ////        if (resourceType.Key == typeof(UserResource))
        ////        {
        ////            data = user.Resource;
        ////        }
        ////        else
        ////        {
        ////            data = user.Extensions.FirstOrDefault(d => d.GetType() == resourceType.Key) ?? Activator.CreateInstance(resourceType.Key) ?? throw new InvalidOperationException();
        ////        }

        ////        foreach (var property in resourceType)
        ////        {
        ////            entityEntry.Property(property.EntityPropertyName).CurrentValue = property.GetResourceValue(data);
        ////        }
        ////    }
        ////}
    }
}