using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IClaimsService _claimsService;
        private readonly ImmutableList<IExternalLoginEvent> _loginEvents;
        private readonly IdentityOptions _options;
        private readonly IInvitationService _invitationService;
        private readonly ILinkUserService _linkUserService;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IUserService _userService;

        public IdentityService(
            IUserService userService,
            IPasswordValidator passwordValidator,
            IClaimsService claimsService,
            IEnumerable<IExternalLoginEvent> loginEvents,
            IOptionsSnapshot<IdentityOptions> options,
            IInvitationService invitationService,
            ILinkUserService linkUserService = null)
        {
            _userService = userService;
            _passwordValidator = passwordValidator;
            _claimsService = claimsService;
            _loginEvents = loginEvents.ToImmutableList();
            _options = options.Value;
            _invitationService = invitationService;
            _linkUserService = linkUserService;
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

            var c = await _claimsService.GetClaimsAsync(identityDataParameters);
            claims.AddRange(c);

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

        public async Task<ClaimsIdentity> LoginExternalAsync(IExternalLoginData externalLoginData)
        {
            foreach (var item in _loginEvents)
            {
                await item.BeginLoginAsync(externalLoginData).ConfigureAwait(false);
            }

            var provider = externalLoginData.LoginRegistration.Id;
            var nameIdentifier = await externalLoginData.GetExternalIdentifierAsync().ConfigureAwait(false);

            var user = await _userService.GetUserByExternalIdAsync(provider, nameIdentifier).ConfigureAwait(false);

            if (externalLoginData.InvitationCode != null)
            {
                var supported = await _invitationService.IsSupportedAsync().ConfigureAwait(false);

                if (!supported || _linkUserService == null)
                {
                    throw new AuthenticationException(Constants.InvitationNotSupported);
                }

                if (user != null)
                {
                    throw new AuthenticationException(Constants.ExternalAccountAlreadyLinkedError);
                }

                var invitation = await _invitationService.RedeemInvitationAsync(externalLoginData.InvitationCode).ConfigureAwait(false);

                user = await _userService.GetUserByIdAsync(invitation.UserId);
                await _linkUserService.LinkUserAsync(user, externalLoginData).ConfigureAwait(false);
            }
            else if (user == null)
            {
                foreach (var item in _loginEvents)
                {
                    await item.UnknownLoginAsync(externalLoginData);
                }

                if (_loginEvents.Any())
                {
                    user = await _userService.GetUserByExternalIdAsync(provider, nameIdentifier).ConfigureAwait(false);
                }

                if (user == null)
                {
                    throw new AuthenticationException(Constants.ExternalAccountNotLinked);
                }
            }

            foreach (var item in _loginEvents)
            {
                await item.LoginSuccessAsync(externalLoginData, user).ConfigureAwait(false);
            }

            var result = await GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);

            return result;
        }

        public virtual Task<ClaimsIdentity> GetClaimsIdentityFromUserAsync(IUser user)
        {
            var identity = new ClaimsIdentity(_options.AuthenticationScheme);

            identity.AddClaim(new Claim(Constants.Claims.Id, user.Identity));
            identity.AddClaim(new Claim(Constants.Claims.Upn, user.Name));
            if (!string.IsNullOrWhiteSpace(user.DefaultTenantKey))
            {
                identity.AddClaim(new Claim(Constants.Claims.DefaultTenant, user.DefaultTenantKey));
            }

            return Task.FromResult(identity);
        }
    }
}