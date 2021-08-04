using System;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityUserService<TContext> : IUserService, IDefaultTenantService, ILinkUserService
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

            return await ToUser(user);
        }

        public virtual async Task<IUser> GetUserByIdAsync(string userId)
        {
            var userSet = GetUserQuery();
            var id = Guid.Parse(userId);

            var user = await userSet.Where(p => p.Id == id).SingleOrDefaultAsync();

            return await ToUser(user);
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

            return await ToUser(user);
        }

        public async Task LinkUserAsync(IUser user, IExternalLoginData loginData)
        {
            var externalId = await loginData.GetExternalIdentifierAsync();
            var userId = Guid.Parse(user.Identity);
            var providerId = Guid.Parse(loginData.LoginRegistration.Id);

            var set = _context.Set<AuthenticationProviderRightHolder>();
            var link = new AuthenticationProviderRightHolder
            {
                RightHolderId = userId,
                ExternalIdentifier = externalId,
                ProviderId = providerId,
            };

            await set.AddAsync(link).ConfigureAwait(false);

            await _context.SaveChangesAsync().ConfigureAwait(false);
        }

        public virtual async Task SetDefaultTenantAsync(string identifier, string tenantKey)
        {
            var userSet = GetUserQuery();
            var userId = Guid.Parse(identifier);

            var user = await userSet.Where(p => p.Id == userId).SingleOrDefaultAsync().ConfigureAwait(false);

            if (Guid.TryParse(tenantKey, out var tenantGuid))
            {
                user.DefaultTenantId = tenantGuid;

                await _context.SaveChangesAsync();
            }
        }

        public async Task UnlinkUserAsync(IUser user, string providerId)
        {
            var userId = Guid.Parse(user.Identity);
            var providerGuid = Guid.Parse(providerId);

            var query = _context.Set<AuthenticationProviderRightHolder>();

            var link = await query.Where(p => p.RightHolderId == userId && p.ProviderId == providerGuid).FirstOrDefaultAsync();
            if (link != null)
            {
                _context.Remove(link);

                await _context.SaveChangesAsync();
            }
        }

        protected virtual IQueryable<User> GetUserQuery()
        {
            return _context.Set<User>().Where(p => !p.IsDisabled);
        }

        private async Task<Data.User> ToUser(User user)
        {
            if (user == null)
            {
                return null;
            }

            var providers = await _context.Set<Model.AuthenticationProviderRightHolder>().Where(p => p.RightHolderId == user.Id).Select(p => p.ProviderId).ToListAsync();

            return new Data.User
            {
                Identity = user.Id.ToString("N"),
                DefaultTenantKey = user.DefaultTenantId?.ToString("N"),
                ForceChangePassword = user.ForceChangePassword,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
                LinkedProviders = providers.Select(p => p.ToString("N")).ToImmutableList(),
            };
        }
    }
}