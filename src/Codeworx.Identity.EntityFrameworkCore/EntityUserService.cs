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
    public class EntityUserService<TContext> : IUserService, IDefaultTenantService, ILinkUserService, IFailedLoginService
        where TContext : DbContext
    {
        private readonly TContext _context;

        public EntityUserService(TContext context)
        {
            _context = context;
        }

        public async Task<string> GetProviderValueAsync(ClaimsIdentity user, string provider)
        {
            var userId = Guid.Parse(user.GetUserId());

            var authenticationProviderSet = _context.Set<AuthenticationProvider>();
            var authenticationProviderRightHolderSet = _context.Set<AuthenticationProviderRightHolder>();
            var providerId = Guid.Parse(provider);

            var authenticationProvider = await authenticationProviderSet.SingleOrDefaultAsync(p => p.Id == providerId);
            var authenticationProviderId = authenticationProvider?.Id ?? throw new AuthenticationProviderException(provider);

            var result = await authenticationProviderRightHolderSet
                                .Where(p => p.RightHolderId == userId && p.ProviderId == authenticationProviderId)
                                .Select(p => p.ExternalIdentifier)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

            return result;
        }

        public virtual async Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            IQueryable<User> userQuery = GetUserQuery();
            var authenticationProviderSet = _context.Set<AuthenticationProvider>();
            var providerId = Guid.Parse(provider);

            var authenticationProvider = await authenticationProviderSet.SingleOrDefaultAsync(p => p.Id == providerId);
            var authenticationProviderId = authenticationProvider?.Id ?? throw new AuthenticationProviderException(provider);

            var user = await userQuery.SingleOrDefaultAsync(p => p.Providers.Any(a => a.ProviderId == authenticationProviderId && a.ExternalIdentifier == nameIdentifier));

            bool hasMfaRegistration = false;

            if (user != null)
            {
                hasMfaRegistration = await HasMfaRegistrationAsync(user);
            }

            return await ToUserAsync(user, hasMfaRegistration);
        }

        public virtual async Task<IUser> GetUserByIdAsync(string userId)
        {
            var userSet = GetUserQuery();
            var id = Guid.Parse(userId);

            var user = await userSet.Where(p => p.Id == id).SingleOrDefaultAsync();
            bool hasMfaRegistration = await HasMfaRegistrationAsync(user);

            return await ToUserAsync(user, hasMfaRegistration);
        }

        public virtual async Task<IUser> GetUserByIdentityAsync(ClaimsIdentity identity)
        {
            var identifier = identity.GetUserId();

            return await GetUserByIdAsync(identifier);
        }

        public virtual async Task<IUser> GetUserByNameAsync(string username)
        {
            var userSet = GetUserQuery();

            var user = await userSet.Where(p => p.Name == username).SingleOrDefaultAsync();
            bool hasMfaRegistration = await HasMfaRegistrationAsync(user);

            return await ToUserAsync(user, hasMfaRegistration);
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

        public async Task<IUser> ResetFailedLoginsAsync(IUser user)
        {
            var userSet = GetUserQuery();
            var id = Guid.Parse(user.Identity);

            var databaseUser = await userSet.Where(p => p.Id == id).SingleOrDefaultAsync().ConfigureAwait(false);

            databaseUser.FailedLoginCount = 0;

            await _context.SaveChangesAsync().ConfigureAwait(false);

            return await ToUserAsync(databaseUser, user.HasMfaRegistration);
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

        public async Task SetFailedLoginAsync(IUser user)
        {
            var userSet = GetUserQuery();
            var id = Guid.Parse(user.Identity);

            var databaseUser = await userSet.Where(p => p.Id == id).SingleOrDefaultAsync().ConfigureAwait(false);

            // With enough patience an attacker could cause the failed login counter to wrap around and become valid again.
            if (databaseUser.FailedLoginCount < int.MaxValue)
            {
                databaseUser.FailedLoginCount++;
            }

            databaseUser.LastFailedLoginAttempt = DateTime.UtcNow;

            await _context.SaveChangesAsync().ConfigureAwait(false);
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
            IQueryable<User> query = _context.Set<User>().Where(p => !p.IsDisabled);

            return query;
        }

        private async Task<bool> HasMfaRegistrationAsync(User user)
        {
            return await _context.Set<AuthenticationProviderRightHolder>()
                                        .Where(p => p.RightHolderId == user.Id && p.Provider.Usage == LoginProviderType.MultiFactor)
                                        .AnyAsync();
        }

        private async Task<Data.User> ToUserAsync(User user, bool hasMfaRegistration)
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
                ConfirmationPending = user.ConfirmationPending,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
                LinkedProviders = providers.Select(p => p.ToString("N")).ToImmutableList(),
                FailedLoginCount = user.FailedLoginCount,
                HasMfaRegistration = hasMfaRegistration,
            };
        }
    }
}