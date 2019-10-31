using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityUserService<TContext> : IUserService, IDefaultTenantService
        where TContext : DbContext
    {
        private readonly TContext _context;

        public EntityUserService(TContext context)
        {
            _context = context;
        }

        public Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            throw new NotImplementedException();
        }

        public virtual async Task<IUser> GetUserByIdentifierAsync(ClaimsIdentity identity)
        {
            var data = identity.ToIdentityData();

            var userSet = _context.Set<User>();
            var id = Guid.Parse(data.Identifier);

            var user = await userSet.Where(p => p.Id == id).SingleOrDefaultAsync();

            return user;
        }

        public virtual async Task<IUser> GetUserByNameAsync(string username)
        {
            var userSet = _context.Set<User>();

            var user = await userSet.Where(p => p.Name == username).SingleOrDefaultAsync();

            return user;
        }

        public async Task SetDefaultTenantAsync(string identifier, string tenantKey)
        {
            var userSet = _context.Set<User>();
            var userId = Guid.Parse(identifier);

            var user = await userSet.Where(p => p.Id == userId).SingleOrDefaultAsync();

            if (Guid.TryParse(tenantKey, out var tenantGuid))
            {
                user.DefaultTenantId = tenantGuid;

                await _context.SaveChangesAsync();
            }
        }
    }
}