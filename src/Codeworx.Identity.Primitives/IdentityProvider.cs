using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class IdentityProvider : IIdentityProvider
    {
        private readonly ImmutableList<IClaimsProvider> _claimsProviders;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IProviderSetup _providerSetup;
        private readonly ITenantProvider _tenantProvider;
        private readonly IUserProvider _userProvider;

        public IdentityProvider(IUserProvider userProvider, IProviderSetup providerSetup, IPasswordValidator passwordValidator, ITenantProvider tenantProvider, IEnumerable<IClaimsProvider> claimsProvider)
        {
            _userProvider = userProvider;
            _providerSetup = providerSetup;
            _passwordValidator = passwordValidator;
            _tenantProvider = tenantProvider;
            _claimsProviders = ImmutableList.CreateRange(claimsProvider);
        }

        public async Task<IdentityData> GetIdentityAsync(string identity, string tenantKey)
        {
            var user = await _userProvider.GetUserByIdentifierAsync(identity);

            if (user == null)
            {
                throw new AuthenticationException();
            }

            var result = await GetIdentityAsync(user, tenantKey);

            return result;
        }

        public async Task<IdentityData> LoginAsync(string username, string password)
        {
            var user = await _userProvider.GetUserByNameAsync(username);
            if (user == null)
            {
                throw new AuthenticationException();
            }

            if (!await _passwordValidator.Validate(user, password))
            {
                throw new AuthenticationException();
            }

            return await GetIdentityAsync(user);
        }

        public async Task<IdentityData> LoginExternalAsync(string provider, string nameIdentifier)
        {
            var identity = await _providerSetup.GetUserIdentity(provider, nameIdentifier);
            var user = await _userProvider.GetUserByIdentifierAsync(identity);

            if (user == null)
            {
                throw new AuthenticationException();
            }

            var result = await GetIdentityAsync(user);
            return result;
        }

        protected virtual async Task<IdentityData> GetIdentityAsync(IUser user, string tenantKey = null)
        {
            var tenants = await _tenantProvider.GetTenantsAsync(user);

            var claims = new List<AssignedClaim>();
            foreach (var cp in _claimsProviders)
            {
                var c = await cp.GetClaimsAsync(user, tenantKey ?? tenants.SingleOrDefault()?.Key);
                claims.AddRange(c);
            }

            var result = new IdentityData(user.Identity, user.Name, tenants, claims, tenantKey);

            return result;
        }
    }
}