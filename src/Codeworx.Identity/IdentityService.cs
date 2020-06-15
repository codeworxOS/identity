using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly ImmutableList<IClaimsService> _claimsProviders;
        private readonly IdentityOptions _options;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IUserService _userService;

        public IdentityService(
            IUserService userService,
            IPasswordValidator passwordValidator,
            IEnumerable<IClaimsService> claimsProvider,
            IOptionsSnapshot<IdentityOptions> options)
        {
            _userService = userService;
            _passwordValidator = passwordValidator;
            _claimsProviders = ImmutableList.CreateRange(claimsProvider);
            _options = options.Value;
        }

        public async Task<IdentityData> GetIdentityAsync(IIdentityDataParameters identityDataParameters)
        {
            if (identityDataParameters is null)
            {
                throw new System.ArgumentNullException(nameof(identityDataParameters));
            }

            var currentUser = await _userService.GetUserByIdentifierAsync(identityDataParameters.User);

            if (currentUser == null)
            {
                throw new AuthenticationException();
            }

            var claims = new List<AssignedClaim>();

            foreach (var cp in _claimsProviders)
            {
                var c = await cp.GetClaimsAsync(currentUser, identityDataParameters);
                claims.AddRange(c);
            }

            var result = new IdentityData(identityDataParameters.ClientId, currentUser.Identity, currentUser.Name, claims);

            return result;
        }

        public async Task<ClaimsIdentity> LoginAsync(string username, string password)
        {
            var user = await _userService.GetUserByNameAsync(username);
            if (user == null)
            {
                throw new AuthenticationException();
            }

            if (!await _passwordValidator.Validate(user, password))
            {
                throw new AuthenticationException();
            }

            return await GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);
        }

        public async Task<ClaimsIdentity> LoginExternalAsync(string provider, string nameIdentifier)
        {
            var user = await _userService.GetUserByExternalIdAsync(provider, nameIdentifier).ConfigureAwait(false);

            if (user == null)
            {
                throw new AuthenticationException();
            }

            var result = await GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);

            return result;
        }

        public virtual Task<ClaimsIdentity> GetClaimsIdentityFromUserAsync(IUser user)
        {
            var identity = new ClaimsIdentity(_options.AuthenticationScheme);

            identity.AddClaim(new Claim(Constants.Claims.Id, user.Identity));
            identity.AddClaim(new Claim(Constants.Claims.Name, user.Name));
            if (!string.IsNullOrWhiteSpace(user.DefaultTenantKey))
            {
                identity.AddClaim(new Claim(Constants.Claims.DefaultTenant, user.DefaultTenantKey));
            }

            return Task.FromResult(identity);
        }
    }
}