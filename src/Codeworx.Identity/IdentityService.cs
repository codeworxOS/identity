using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Security.Principal;
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
        private readonly IdentityServerOptions _serverOptions;
        private readonly ImmutableList<IExternalLoginEvent> _loginEvents;
        private readonly IdentityOptions _options;
        private readonly IInvitationService _invitationService;
        private readonly IStringResources _stringResources;
        private readonly ILoginDelayService _loginDelayService;
        private readonly IFailedLoginService _failedLoginService;
        private readonly ILinkUserService _linkUserService;
        private readonly IPasswordValidator _passwordValidator;
        private readonly IUserService _userService;

        public IdentityService(
            IUserService userService,
            IPasswordValidator passwordValidator,
            IClaimsService claimsService,
            IEnumerable<IExternalLoginEvent> loginEvents,
            IOptionsSnapshot<IdentityOptions> options,
            IdentityServerOptions serverOptions,
            IInvitationService invitationService,
            IStringResources stringResources,
            ILoginDelayService loginDelayService,
            IFailedLoginService failedLoginService = null,
            ILinkUserService linkUserService = null)
        {
            _userService = userService;
            _passwordValidator = passwordValidator;
            _claimsService = claimsService;
            _serverOptions = serverOptions;
            _loginEvents = loginEvents.ToImmutableList();
            _options = options.Value;
            _invitationService = invitationService;
            _stringResources = stringResources;
            _loginDelayService = loginDelayService;
            _failedLoginService = failedLoginService;
            _linkUserService = linkUserService;
        }

        public async Task<IdentityData> GetIdentityAsync(IIdentityDataParameters identityDataParameters)
        {
            if (identityDataParameters is null)
            {
                throw new System.ArgumentNullException(nameof(identityDataParameters));
            }

            var currentUser = identityDataParameters.IdentityUser;

            if (currentUser == null)
            {
                var message = _stringResources.GetResource(StringResource.DefaultAuthenticationError);
                throw new AuthenticationException(message);
            }

            var hasMfa = identityDataParameters.User.HasClaim(Constants.Claims.Amr, Constants.OpenId.Amr.Mfa);

            if (!hasMfa && identityDataParameters.MfaFlowMode == MfaFlowMode.Enabled)
            {
                if (identityDataParameters.Client.AuthenticationMode == AuthenticationMode.Mfa)
                {
                    identityDataParameters.Throw(Constants.OpenId.Error.MfaAuthenticationRequired, Constants.OAuth.ClientIdName);
                }

                if (currentUser.AuthenticationMode == AuthenticationMode.Mfa)
                {
                    identityDataParameters.Throw(Constants.OpenId.Error.MfaAuthenticationRequired, Constants.Claims.Subject);
                }

                if (identityDataParameters.Scopes.Contains(Constants.Scopes.Mfa))
                {
                    identityDataParameters.Throw(Constants.OpenId.Error.MfaAuthenticationRequired, Constants.Claims.Subject);
                }
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
            if (user == null || user.PasswordHash == null)
            {
                await _loginDelayService.DelayAsync();
                var message = _stringResources.GetResource(StringResource.DefaultAuthenticationError);
                throw new AuthenticationException(message);
            }

            if (_options.MaxFailedLogins.HasValue && user.FailedLoginCount > _options.MaxFailedLogins.Value)
            {
                var message = _stringResources.GetResource(StringResource.MaxFailedLoginAttemptsReached);
                throw new AuthenticationException(message);
            }

            var sw = new Stopwatch();
            sw.Start();
            if (!await _passwordValidator.Validate(user, password))
            {
                if (_failedLoginService != null)
                {
                    await _failedLoginService.SetFailedLoginAsync(user).ConfigureAwait(false);
                }

                sw.Stop();
                _loginDelayService.Record(sw.Elapsed);

                var message = _stringResources.GetResource(StringResource.DefaultAuthenticationError);
                throw new AuthenticationException(message);
            }
            else
            {
                sw.Stop();
                _loginDelayService.Record(sw.Elapsed);
            }

            if (_failedLoginService != null && user.FailedLoginCount > 0)
            {
                user = await _failedLoginService.ResetFailedLoginsAsync(user);
            }

            var result = await GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);

            if (user.ForceChangePassword)
            {
                result.AddClaim(new Claim(Constants.Claims.ForceChangePassword, "true"));
            }

            return result;
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
            var identity = new ClaimsIdentity(_serverOptions.AuthenticationScheme);

            identity.AddClaim(new Claim(Constants.Claims.Id, user.Identity));
            identity.AddClaim(new Claim(Constants.Claims.Upn, user.Name));
            identity.AddClaim(new Claim(Constants.Claims.Session, Guid.NewGuid().ToString("N")));

            if (user.AuthenticationMode == AuthenticationMode.Mfa)
            {
                identity.AddClaim(new Claim(Constants.Claims.ForceMfaLogin, "true"));
            }

            if (user.ConfirmationPending)
            {
                identity.AddClaim(new Claim(Constants.Claims.ConfirmationPending, "true"));
            }

            if (!string.IsNullOrWhiteSpace(user.DefaultTenantKey))
            {
                identity.AddClaim(new Claim(Constants.Claims.DefaultTenant, user.DefaultTenantKey));
            }

            return Task.FromResult(identity);
        }
    }
}