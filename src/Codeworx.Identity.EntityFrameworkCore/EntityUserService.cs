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

        public virtual async Task<string> GetProviderValueAsync(string userId, string provider)
        {
            var user = Guid.Parse(userId);

            var authenticationProviderSet = _context.Set<AuthenticationProvider>();
            var providerId = Guid.Parse(provider);

            var query = from a in authenticationProviderSet
                        from ar in a.RightHolders.Where(p => p.RightHolderId == user).DefaultIfEmpty()
                        where a.Id == providerId
                        select new { ProviderId = a.Id, ar.ExternalIdentifier };

            var result = await query.FirstOrDefaultAsync().ConfigureAwait(false);

            result = result ?? throw new AuthenticationProviderException(provider);

            return result.ExternalIdentifier;
        }

        public virtual async Task<IUser> GetUserByExternalIdAsync(string provider, string nameIdentifier)
        {
            IQueryable<User> userQuery = GetUserQuery();
            var providerId = Guid.Parse(provider);

            userQuery = userQuery.Where(p => p.Providers.Any(a => a.ProviderId == providerId && a.ExternalIdentifier == nameIdentifier));

            var result = await ToUserAsync(userQuery).ConfigureAwait(false);

            return result;
        }

        public virtual async Task<IUser> GetUserByIdAsync(string userId)
        {
            IQueryable<User> userQuery = GetUserQuery();
            var id = Guid.Parse(userId);

            userQuery = userQuery.Where(p => p.Id == id);
            return await ToUserAsync(userQuery).ConfigureAwait(false);
        }

        public virtual async Task<IUser> GetUserByIdentityAsync(ClaimsIdentity identity)
        {
            var identifier = identity.GetUserId();

            return await GetUserByIdAsync(identifier).ConfigureAwait(false);
        }

        public virtual async Task<IUser> GetUserByNameAsync(string username)
        {
            IQueryable<User> userQuery = GetUserQuery();

            userQuery = userQuery.Where(p => p.Name == username);

            return await ToUserAsync(userQuery).ConfigureAwait(false);
        }

        public virtual async Task LinkUserAsync(IUser user, IExternalLoginData loginData)
        {
            var externalId = await loginData.GetExternalIdentifierAsync().ConfigureAwait(false);
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

        public virtual async Task<IUser> ResetFailedLoginsAsync(IUser user)
        {
            IQueryable<User> userQuery = GetUserQuery();
            var id = Guid.Parse(user.Identity);

            userQuery = userQuery.Where(p => p.Id == id);

            return await ToUserAsync(userQuery, true);
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

        public virtual async Task SetFailedLoginAsync(IUser user)
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

        public virtual async Task UnlinkUserAsync(IUser user, string providerId)
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

        private async Task<Data.User> ToUserAsync(IQueryable<User> query, bool resetFailedLoginCount = false)
        {
            var result = await query.Select(u => new
            {
                User = u,
                Providers = u.Providers.Select(p => new { p.Provider.Usage, p.ProviderId }).ToList(),
            })
            .SingleOrDefaultAsync().ConfigureAwait(false);

            if (result == null)
            {
                return null;
            }

            var user = result.User;

            if (resetFailedLoginCount)
            {
                user.FailedLoginCount = 0;
                await _context.SaveChangesAsync().ConfigureAwait(false);
            }

            return new Data.User
            {
                Identity = user.Id.ToString("N"),
                DefaultTenantKey = user.DefaultTenantId?.ToString("N"),
                ForceChangePassword = user.ForceChangePassword,
                ConfirmationPending = user.ConfirmationPending,
                Name = user.Name,
                PasswordHash = user.PasswordHash,
                LinkedMfaProviders = result.Providers.Where(p => p.Usage == LoginProviderType.MultiFactor).Select(p => p.ProviderId.ToString("N")).ToImmutableList(),
                LinkedLoginProviders = result.Providers.Where(p => p.Usage == LoginProviderType.Login).Select(p => p.ProviderId.ToString("N")).ToImmutableList(),
                FailedLoginCount = user.FailedLoginCount,
                HasMfaRegistration = result.Providers.Any(p => p.Usage == LoginProviderType.MultiFactor),
                AuthenticationMode = user.AuthenticationMode,
            };
        }
    }
}