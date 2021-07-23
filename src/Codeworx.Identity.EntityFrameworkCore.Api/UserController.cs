using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    [Route("api/identity/users")]
    public class UserController
    {
        private readonly IContextWrapper _db;

        public UserController(IContextWrapper db)
        {
            _db = db;
        }

        [HttpPost]
        public async Task<UserData> InsertUsersAsync([FromBody] UserInsertData user)
        {
            var entity = new User
            {
                Id = Guid.NewGuid(),
                Created = DateTime.UtcNow,
                Name = user.Login,
                ForceChangePassword = user.ForceChangePassword,
                IsDisabled = user.IsDisabled,
                DefaultTenantId = user.DefaultTenantId,
            };

            _db.Context.Add(entity);
            var entry = _db.Context.Entry(entity);

            entry.UpdateAdditionalProperties(user);

            await _db.Context.SaveChangesAsync();

            return await GetUserByIdAsync(entity.Id);
        }

        [HttpPut("{id}/tenant/{tenantId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task AssignTenantAsync(Guid id, Guid tenantId)
        {
            using (var transaction = await _db.Context.Database.BeginTransactionAsync())
            {
                var groupExists = await _db.Context.Set<User>().Where(p => p.Id == id).AnyAsync();
                var tenantExists = await _db.Context.Set<Tenant>().Where(p => p.Id == tenantId).AnyAsync();

                if (!groupExists || !tenantExists)
                {
                    // TODO return 404;
                }

                var entity = new TenantUser { TenantId = tenantId, RightHolderId = id };

                _db.Context.Add(entity);

                await _db.Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        [HttpDelete("{id}/tenant/{tenantId}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task RemoveMemberAsync(Guid id, Guid tenantId)
        {
            using (var transaction = await _db.Context.Database.BeginTransactionAsync())
            {
                var assignment = await _db.Context.Set<TenantUser>().Where(p => p.TenantId == tenantId && p.RightHolderId == id).FirstOrDefaultAsync();

                if (assignment == null)
                {
                    // TODO return 404;
                }

                _db.Context.Remove(assignment);

                await _db.Context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
        }

        [HttpPut]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<UserData> UpdateUsersAsync([FromBody] UserUpdateData user)
        {
            var entity = await _db.Context.Set<User>().Where(p => p.Id == user.Id).FirstOrDefaultAsync();

            if (entity == null)
            {
                // TODO return 404;
            }

            entity.ForceChangePassword = user.ForceChangePassword;
            entity.DefaultTenantId = user.DefaultTenantId;
            entity.IsDisabled = user.IsDisabled;
            entity.Name = user.Login;

            var entry = _db.Context.Entry(entity);

            entry.UpdateAdditionalProperties(user);

            await _db.Context.SaveChangesAsync();

            return await GetUserByIdAsync(entity.Id);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task DeleteUsersAsync(Guid id)
        {
            var entity = await _db.Context.Set<User>().Where(p => p.Id == id).FirstOrDefaultAsync();

            if (entity == null)
            {
                // TODO return 404;
            }

            var entry = _db.Context.Remove(entity);

            await _db.Context.SaveChangesAsync();
        }

        [HttpGet]
        public async Task<IEnumerable<UserListData>> GetUsersAsync([FromQuery] Guid? tenantId = null)
        {
            IQueryable<User> query = _db.Context.Set<User>();

            if (tenantId.HasValue)
            {
                query = query.Where(p => p.Tenants.Any(x => x.TenantId == tenantId.Value));
            }

            var users = await query.ToListAsync();
            var result = new List<UserListData>();

            foreach (var user in users)
            {
                var data = new UserListData
                {
                    Id = user.Id,
                    Login = user.Name,
                    Created = user.Created,
                    DefaultTenantId = user.DefaultTenantId,
                    IsDisabled = user.IsDisabled,
                };

                _db.Context.Entry(user).MapAdditionalProperties(data);

                result.Add(data);
            }

            return result;
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<UserData> GetUserByIdAsync(Guid id, [FromQuery] string expand = null)
        {
            var set = _db.Context.Set<User>();
            var user = await set.Where(p => p.Id == id).FirstOrDefaultAsync();

            var expands = expand?.Split(",")?.Select(p => p.Trim())?.ToList() ?? new List<string>();

            if (user == null)
            {
                // TODO return 404;
            }

            var result = new List<UserData>();

            var entry = _db.Context.Entry(user);
            var data = new UserData
            {
                Id = user.Id,
                Login = user.Name,
                Created = user.Created,
                DefaultTenantId = user.DefaultTenantId,
                FailedLoginCount = user.FailedLoginCount,
                ForceChangePassword = user.ForceChangePassword,
                IsDisabled = user.IsDisabled,
                LastFailedLoginAttempt = user.LastFailedLoginAttempt,
                PasswordChanged = user.PasswordChanged,
            };

            if (expands.Contains(nameof(UserData.Tenants).ToLower()))
            {
                var tenantInfos = await _db.Context.Set<TenantUser>()
                    .Where(p => p.RightHolderId == user.Id)
                    .Select(p => new TenantInfoData
                    {
                        Id = p.Tenant.Id,
                        DisplayName = p.Tenant.Name,
                    }).ToListAsync();

                foreach (var tenant in tenantInfos)
                {
                    data.Tenants.Add(tenant);
                }
            }

            if (expands.Contains(nameof(UserData.Groups).ToLower()))
            {
                var groups = await _db.Context.Set<RightHolderGroup>()
                    .Where(p => p.RightHolderId == user.Id)
                    .Select(p => new GroupInfoData
                    {
                        Id = p.Group.Id,
                        Name = p.Group.Name,
                    }).ToListAsync();

                foreach (var group in groups)
                {
                    data.Groups.Add(group);
                }
            }

            entry.MapAdditionalProperties(data);

            return data;
        }
    }
}
