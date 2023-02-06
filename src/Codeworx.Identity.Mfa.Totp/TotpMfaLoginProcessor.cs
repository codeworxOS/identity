using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Options;
using OtpNet;

namespace Codeworx.Identity.Mfa.Totp
{
    public class TotpMfaLoginProcessor : ILoginProcessor
    {
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly ILinkUserService _linkUserService;
        private readonly IdentityOptions _options;
        private readonly IStringResources _stringResources;
        private readonly IUserService _userService;

        public TotpMfaLoginProcessor(
            IOptionsSnapshot<IdentityOptions> options,
            IUserService userService,
            IBaseUriAccessor baseUriAccessor,
            IStringResources stringResources,
            ILinkUserService linkUserService = null)
        {
            _options = options.Value;
            _userService = userService;
            _baseUriAccessor = baseUriAccessor;
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
                case ProviderRequestType.MfaList:
                    return Task.FromResult<ILoginRegistrationInfo>(GetMfaListInfo(request, registration));
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
                if (loginRequest.Action == MfaAction.Register)
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
                        if (user.LinkedMfaProviders.Contains(configuration.Id))
                        {
                            var message = _stringResources.GetResource(StringResource.GenericLoginError);
                            throw new AuthenticationException(message);
                        }

                        await _linkUserService.LinkUserAsync(user, new TotpLoginData(configuration, loginRequest.SharedSecret)).ConfigureAwait(false);
                        ClaimsIdentity identity = GenerateMfaIdentity();

                        return new SignInResponse(identity, loginRequest.ReturnUrl, AuthenticationMode.Mfa);
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
                        var identity = GenerateMfaIdentity();

                        return new SignInResponse(identity, loginRequest.ReturnUrl, AuthenticationMode.Mfa, loginRequest.RememberMe);
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

        private ClaimsIdentity GenerateMfaIdentity()
        {
            var identity = new ClaimsIdentity(_options.MfaAuthenticationScheme);
            identity.AddClaim(new Claim(Constants.Claims.Amr, Constants.OpenId.Amr.Mfa));
            identity.AddClaim(new Claim(Constants.Claims.Amr, Constants.OpenId.Amr.Otp));
            return identity;
        }

        private ILoginRegistrationInfo GetMfaListInfo(ProviderRequest request, ILoginRegistration registration)
        {
            string description = _stringResources.GetResource(StringResource.MfaListRegisterTotp);

            if (request.User.LinkedMfaProviders.Contains(registration.Id))
            {
                description = _stringResources.GetResource(StringResource.OneTimeCodeViaApp);
            }
            else if (request.User.HasMfaRegistration && !request.IsMfaAuthenticated)
            {
                return null;
            }

            request.ProviderErrors.TryGetValue(registration.Id, out var error);

            var builder = new UriBuilder(_baseUriAccessor.BaseUri);
            builder.AppendPath(_options.AccountEndpoint);
            builder.AppendPath("login/mfa");
            builder.AppendPath(registration.Id);
            if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            {
                builder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            }

            return new MfaProviderListInfo(
                registration.Id,
                builder.ToString(),
                description,
                "fa-solid fa-qrcode",
                error);
        }
    }
}
