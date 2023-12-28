using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Binding;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("scim/Users")]
    [AllowAnonymous]
    public class UsersController : Controller
    {
        private readonly DbContext _db;
        private readonly IEnumerable<IUserSchemaProperty> _mappedProperties;

        public UsersController(IContextWrapper contextWrapper, IEnumerable<IUserSchemaProperty> mappedProperties)
        {
            _db = contextWrapper.Context;
            _mappedProperties = mappedProperties;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse> GetUsersAsync([FromQuery] int startIndex, [FromQuery] int count)
        {
            ConfigHelper.ValidateDefaultPagination(ref startIndex, ref count);

            var query = _db.Set<User>().AsNoTracking().OrderBy(c => c.Id).AsQueryable();
            var totalResults = await query.CountAsync();

            if (startIndex > 1)
            {
                query = query.Skip(startIndex - 1);
            }

            if (count > 0)
            {
                query = query.Take(count);
            }

            var users = await query.ToListAsync();

            var result = new List<UserResponse>();

            foreach (var item in users)
            {
                UserResponse response = GenerateUserResponse(item);

                result.Add(response);
            }

            return new ListResponse(startIndex, totalResults, count, result);
        }

        [HttpGet("{userId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> GetUserAsync(Guid userId)
        {
            var user = await _db.Set<User>().AsQueryable().Where(t => t.Id == userId).FirstOrDefaultAsync();

            if (user == null)
            {
                return NotFound();
            }

            UserResponse response = GenerateUserResponse(user);

            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponse>> AddUserAsync([RequestResourceBinder] UserRequest user)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var item = new User
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    ForceChangePassword = true,
                    AuthenticationMode = Login.AuthenticationMode.Login,
                };

                _db.Add(item);

                ApplyEntityChanges(user, item);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = await GetUserAsync(item.Id);
                return CreatedAtAction(nameof(AddUserAsync), response.Value);
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> UpdateUsersAsync(Guid id, [RequestResourceBinder] UserRequest user)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var item = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

                if (item == null)
                {
                    return NotFound();
                }

                ApplyEntityChanges(user, item);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(await GetUserAsync(item.Id));
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

        private UserResponse GenerateUserResponse(User item)
        {
            var userUrl = this.Url.ActionLink(controller: "Users", action: "GetUser", values: new { userId = item.Id.ToString("N") })!;

            var info = new ScimResponseInfo(item.Id.ToString("N"), userUrl, item.Created, item.Created);

            var entityEntry = _db.Entry(item);
            var resource = new UserResource();
            var list = new List<ISchemaResource>();
            foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
            {
                object data;
                if (resourceType.Key == typeof(UserResource))
                {
                    data = resource;
                }
                else
                {
                    if (Activator.CreateInstance(resourceType.Key) is ISchemaResource d)
                    {
                        data = d;
                        list.Add(d);
                    }
                    else
                    {
                        throw new NotSupportedException();
                    }
                }

                foreach (var property in resourceType)
                {
                    property.SetResourceValue(data, entityEntry.Property(property.EntityPropertyName).CurrentValue);
                }
            }

            var response = new UserResponse(info, resource, list.ToArray());
            return response;
        }

        private void ApplyEntityChanges(UserRequest user, User item)
        {
            var entityEntry = _db.Entry(item);

            foreach (var resourceType in _mappedProperties.GroupBy(d => d.ResourceType))
            {
                object data;
                if (resourceType.Key == typeof(UserResource))
                {
                    data = user.Resource;
                }
                else
                {
                    data = user.Extensions.FirstOrDefault(d => d.GetType() == resourceType.Key) ?? Activator.CreateInstance(resourceType.Key) ?? throw new InvalidOperationException();
                }

                foreach (var property in resourceType)
                {
                    entityEntry.Property(property.EntityPropertyName).CurrentValue = property.GetResourceValue(data);
                }
            }
        }
    }
}