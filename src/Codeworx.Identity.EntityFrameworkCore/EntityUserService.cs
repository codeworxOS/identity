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

        public virtual async Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            IQueryable<User> userQuery = GetUserQuery();
            var authenticationProviderSet = _context.Set<AuthenticationProvider>();
            var providerId = Guid.Parse(provider);

            var authenticationProvider = await authenticationProviderSet.SingleOrDefaultAsync(p => p.Id == providerId);
            var authenticationProviderId = authenticationProvider?.Id ?? throw new AuthenticationProviderException(provider);

            var user = await userQuery.SingleOrDefaultAsync(p => p.Providers.Any(a => a.ProviderId == authenticationProviderId && a.ExternalIdentifier == nameIdentifier));

            return ToUser(user);
        }

        public virtual async Task<IUser> GetUserByIdAsync(string userId)
        {
            var userSet = GetUserQuery();
            var id = Guid.Parse(userId);

            var user = await userSet.Where(p => p.Id == id).SingleOrDefaultAsync();

            return ToUser(user);
        }

        public virtual async Task<IUser> GetUserByIdentifierAsync(ClaimsIdentity identity)
        {
            var identifier = identity.GetUserId();

            return await GetUserByIdAsync(identifier);
        }

        public virtual async Task<IUser> GetUserByNameAsync(string username)
        {
            var userSet = GetUserQuery();

            var user = await userSet.Where(p => p.Name == username).SingleOrDefaultAsync();

            return ToUser(user);
        }

        public virtual async Task SetDefaultTenantAsync(string identifier, string tenantKey)
        {
            var userSet = GetUserQuery();
            var userId = Guid.Parse(identifier);

            var user = await userSet.Where(p => p.Id == userId).SingleOrDefaultAsync();

            if (Guid.TryParse(tenantKey, out var tenantGuid))
            {
                user.DefaultTenantId = tenantGuid;

                await _context.SaveChangesAsync();
            }
        }

        protected virtual IQueryable<User> GetUserQuery()
        {
            return _context.Set<User>().Where(p => !p.IsDisabled);
        }

        private Data.User ToUser(User user)
        {
            return new Data.User
            {
                Identity = user.Id.ToString("N"),
                DefaultTenantKey = user.DefaultTenantId?.ToString("N"),
                ForceChangePassword = user.ForceChangePassword,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
            };
        }
    }
}