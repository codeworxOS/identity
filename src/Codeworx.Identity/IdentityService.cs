using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity
{
    public class IdentityService : IIdentityService
    {
        private readonly IClaimsService _claimsService;
        private readonly ImmutableList<IExternalLoginEvent> _loginEvents;
        private readonly IdentityOptions _options;
        private readonly IInvitationService _invitationService;
        private readonly IStringResources _stringResources;
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
            IStringResources stringResources,
            ILinkUserService linkUserService = null)
        {
            _userService = userService;
            _passwordValidator = passwordValidator;
            _claimsService = claimsService;
            _loginEvents = loginEvents.ToImmutableList();
            _options = options.Value;
            _invitationService = invitationService;
            _stringResources = stringResources;
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
                var message = _stringResources.GetResource(StringResource.DefaultAuthenticationError);
                throw new AuthenticationException(message);
            }

            var claims = new List<AssignedClaim>();

            var c = await _claimsService.GetClaimsAsync(identityDataParameters);
            claims.AddRange(c);

            var externalTokenKey = identityDataParameters.User.FindFirst(Constants.Claims.ExternalTokenKey)?.Value;

            var result = new IdentityData(identityDataParameters.Client.ClientId, currentUser.Identity, currentUser.Name, claims, externalTokenKey);

            return result;
        }

        public async Task<ClaimsIdentity> LoginAsync(string username, string password)
        {
            var user = await _userService.GetUserByNameAsync(username);
            if (user == null)
            {
                var message = _stringResources.GetResource(StringResource.DefaultAuthenticationError);
                throw new AuthenticationException(message);
            }

            if (!await _passwordValidator.Validate(user, password))
            {
                var message = _stringResources.GetResource(StringResource.DefaultAuthenticationError);
                throw new AuthenticationException(message);
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
                    var message = _stringResources.GetResource(StringResource.InvitationNotSupportedError);
                    throw new AuthenticationException(message);
                }

                if (user != null)
                {
                    var message = _stringResources.GetResource(StringResource.ExternalAccountAlreadyLinkedError);
                    throw new AuthenticationException(message);
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
                    var message = _stringResources.GetResource(StringResource.ExternalAccountNotLinkedError);
                    throw new AuthenticationException(message);
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

            if (user.ForceChangePassword)
            {
                identity.AddClaim(new Claim(Constants.Claims.ForceChangePassword, "true"));
            }

            if (!string.IsNullOrWhiteSpace(user.DefaultTenantKey))
            {
                identity.AddClaim(new Claim(Constants.Claims.DefaultTenant, user.DefaultTenantKey));
            }

            return Task.FromResult(identity);
        }
    }
}