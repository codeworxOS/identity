using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
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
        private DbContext _db;

        public UsersController(IContextWrapper contextWrapper)
        {
            _db = contextWrapper.Context;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse<UserResponse>> GetUsersAsync([FromQuery] int startIndex, [FromQuery] int count)
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
                var info = new ScimResponseInfo(item.Id.ToString("N"), this.Url.ActionLink(controller: "Users")!, DateTime.Today, DateTime.Today);

                var response = new UserResponse(info, new UserResource(), new ISchemaResource[] { });

                result.Add(response);
            }

            return new ListResponse<UserResponse>(startIndex, totalResults, count, result);
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

            var info = new ScimResponseInfo(user.Id.ToString("N"), this.Url.ActionLink(controller: "Users")!, DateTime.Today, DateTime.Today);

            var response = new UserResponse(info, new UserResource(), new ISchemaResource[] { });

            return response;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponse>> AddUserAsync([RequestResourceBinder] UserRequest user)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var entity = new User
                {
                    Id = Guid.NewGuid(),
                    Created = DateTime.UtcNow,
                    ForceChangePassword = true,
                    AuthenticationMode = Login.AuthenticationMode.Login,
                };

                _db.Add(entity);

                // ToDo DI?
                ////entity.ApplyParameters(user, _parameter);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                var response = await GetUserAsync(entity.Id);
                return CreatedAtAction(nameof(AddUserAsync), response.Value);
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> UpdateUsersAsync(Guid id, [RequestResourceBinder] UserResponse user)
        {
            using (var transaction = await _db.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var entity = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

                if (entity == null)
                {
                    return NotFound();
                }

                // ToDo DI?
                ////entity.ApplyParameters(user, _parameter);

                await _db.SaveChangesAsync();

                await transaction.CommitAsync();

                return Ok(await GetUserAsync(entity.Id));
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

        ////[HttpDelete("{id}")]
        ////[ProducesResponseType(StatusCodes.Status204NoContent)]
        ////[ProducesResponseType(StatusCodes.Status404NotFound)]
        ////public async Task<ActionResult> DeleteUsersAsync(Guid id)
        ////{
        ////    var entity = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

        ////    if (entity == null)
        ////    {
        ////        return NotFound();
        ////    }

        ////    var entry = _db.Remove(entity);

        ////    await _db.SaveChangesAsync();

        ////    return NoContent();
        ////}
    }
}