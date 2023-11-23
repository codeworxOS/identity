using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Model;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api
{
    [Route("authProvider/{authProviderId}/scim/Users")]
    [AllowAnonymous]
    public class UsersController : Controller
    {
        private DbContext _db;
        private IEnumerable<ISchemaParameterDescription<User>> _parameter;
        private IOptionsSnapshot<IdentityOptions> _options;
        private IUserService _userService;
        private IConfirmationService _confirmationService;

        public UsersController(IContextWrapper contextWrapper, IEnumerable<ISchemaParameterDescription<User>> parameter, IOptionsSnapshot<IdentityOptions> options, IUserService userService, IConfirmationService confirmationService = null)
        {
            _db = contextWrapper.Context;
            _parameter = parameter;
            _options = options;
            _userService = userService;
            _confirmationService = confirmationService;
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ListResponse<UserResponse>> GetUsersAsync([FromQuery] int startIndex, [FromQuery] int count)
        {
            if (startIndex < 1)
            {
                startIndex = 1;
            }

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
                var data = new UserResponse
                {
                    Id = item.Id,
                };

                data.ApplyParameters(item, _parameter);

                result.Add(data);
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

            var result = new UserResponse
            {
                Id = user.Id,
            };

            result.ApplyParameters(user, _parameter);

            return result;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<UserResponse>> AddUserAsync([FromBody] UserResponse user)
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

                entity.ApplyParameters(user, _parameter);

                _db.Add(entity);
                var entry = _db.Entry(entity);

                await _db.SaveChangesAsync();

                if (_options.Value.EnableAccountConfirmation)
                {
                    if (_confirmationService == null)
                    {
                        // TODO return 412
                        throw new NotSupportedException("Missing IConfirmationService!");
                    }

                    var userData = await _userService.GetUserByIdAsync(entity.Id.ToString("N")).ConfigureAwait(false);

                    await _confirmationService.RequireConfirmationAsync(userData).ConfigureAwait(false);
                }

                transaction.Commit();

                var response = await GetUserAsync(entity.Id);
                return CreatedAtAction(nameof(AddUserAsync), response.Value);
            }
        }

        [HttpPut("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<UserResponse>> UpdateUsersAsync(Guid id, [FromBody] UserResponse user)
        {
            var entity = await _db.Set<User>().Where(p => p.Id == user.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }

            entity.ApplyParameters(user, _parameter);

            await _db.SaveChangesAsync();

            return Ok(await GetUserAsync(entity.Id));
        }

        [HttpPatch("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<UserResponse>> PatchUserAsync(Guid id, [FromBody] PatchOperation patch)
        {
            var entity = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }

            foreach (var operation in patch.Operations)
            {
                // todo apply operations
            }

            await _db.SaveChangesAsync();

            return Ok(await GetUserAsync(entity.Id));
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteUsersAsync(Guid id)
        {
            var entity = await _db.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                return NotFound();
            }

            var entry = _db.Remove(entity);

            await _db.SaveChangesAsync();

            return NoContent();
        }
    }
}