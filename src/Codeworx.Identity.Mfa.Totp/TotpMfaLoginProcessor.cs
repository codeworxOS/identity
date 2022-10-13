using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Options;
using OtpNet;

namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpMfaLoginProcessor : ILoginProcessor
    {
        private readonly IdentityOptions _options;
        private readonly IUserService _userService;
        private readonly IStringResources _stringResources;
        private readonly ILinkUserService _linkUserService;

        public TotpMfaLoginProcessor(
            IOptionsSnapshot<IdentityOptions> options,
            IUserService userService,
            IStringResources stringResources,
            ILinkUserService linkUserService = null)
        {
            _options = options.Value;
            _userService = userService;
            _stringResources = stringResources;
            _linkUserService = linkUserService;
        }

        public Type RequestParameterType => typeof(TotpLoginRequest);

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration)
        {
            string error = null;
            request.ProviderErrors.TryGetValue(registration.Id, out error);

            switch (request.Type)
            {
                case ProviderRequestType.MfaLogin:
                    return Task.FromResult<ILoginRegistrationInfo>(new LoginTotpInfo(request.User, registration.Id, error));
                case ProviderRequestType.MfaRegister:
                    return Task.FromResult<ILoginRegistrationInfo>(new RegisterTotpInfo(request.User, _options, registration.Id, error));
                case ProviderRequestType.Login:
                case ProviderRequestType.Invitation:
                case ProviderRequestType.Profile:
                default:
                    throw new NotSupportedException();
            }
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRegistration configuration, object request)
        {
            if (request is TotpLoginRequest loginRequest)
            {
                if (loginRequest.Action == TotpAction.Register)
                {
                    var key = Base32Encoding.ToBytes(loginRequest.SharedSecret);
                    var totp = new OtpNet.Totp(key);
                    var verified = totp.VerifyTotp(loginRequest.OneTimeCode, out var stepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                    if (verified)
                    {
                        if (_linkUserService == null)
                        {
                            throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("registration not supported!"));
                        }

                        var user = await _userService.GetUserByIdentityAsync(loginRequest.Identity).ConfigureAwait(false);
                        if (user.LinkedProviders.Contains(configuration.Id))
                        {
                            var message = _stringResources.GetResource(StringResource.GenericLoginError);
                            throw new AuthenticationException(message);
                        }

                        await _linkUserService.LinkUserAsync(user, new TotpLoginData(configuration, loginRequest.SharedSecret)).ConfigureAwait(false);

                        return new SignInResponse(loginRequest.Identity, loginRequest.ReturnUrl, AuthenticationMode.Mfa);
                    }
                    else
                    {
                        var message = _stringResources.GetResource(StringResource.InvalidOneTimeCode);
                        throw new AuthenticationException(message);
                    }
                }
                else
                {
                    var sharedSecret = await _userService.GetProviderValueAsync(loginRequest.Identity.GetUserId(), loginRequest.ProviderId).ConfigureAwait(false);

                    if (sharedSecret == null)
                    {
                        var message = _stringResources.GetResource(StringResource.InvalidOneTimeCode);
                        throw new AuthenticationException(message);
                    }

                    var key = Base32Encoding.ToBytes(sharedSecret);
                    var totp = new OtpNet.Totp(key);
                    var verified = totp.VerifyTotp(loginRequest.OneTimeCode, out var stepMatched, VerificationWindow.RfcSpecifiedNetworkDelay);

                    if (verified)
                    {
                        var identity = new ClaimsIdentity(_options.MfaAuthenticationScheme);
                        identity.AddClaim(new Claim(Constants.Claims.Amr, Constants.OpenId.Amr.Mfa));
                        identity.AddClaim(new Claim(Constants.Claims.Amr, Constants.OpenId.Amr.Otp));

                        return new SignInResponse(identity, loginRequest.ReturnUrl, AuthenticationMode.Mfa);
                    }
                    else
                    {
                        var message = _stringResources.GetResource(StringResource.InvalidOneTimeCode);
                        throw new AuthenticationException(message);
                    }
                }
            }

            throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("Wrong request parameter!"));
        }
    }
}
