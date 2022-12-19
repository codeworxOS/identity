using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Model;
using Codeworx.Identity.Notification;
using Codeworx.Identity.Resources;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaLoginProcessor : ILoginProcessor
    {
        private static Random _random;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly ILinkUserService _linkUserService;
        private readonly INotificationService _notification;
        private readonly IdentityOptions _options;
        private readonly IStringResources _stringResources;
        private readonly IUserService _userService;

        static MailMfaLoginProcessor()
        {
            _random = new Random();
        }

        public MailMfaLoginProcessor(
            IUserService userService,
            IStringResources stringResources,
            IBaseUriAccessor baseUriAccessor,
            IOptionsSnapshot<IdentityOptions> options,
            ILinkUserService linkUserService = null,
            INotificationService notification = null)
        {
            _options = options.Value;
            _userService = userService;
            _stringResources = stringResources;
            _baseUriAccessor = baseUriAccessor;
            _linkUserService = linkUserService;
            _notification = notification;
        }

        public Type RequestParameterType { get; } = typeof(MailLoginRequest);

        public async Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration registration)
        {
            string error = null;
            request.ProviderErrors.TryGetValue(registration.Id, out error);

            switch (request.Type)
            {
                case ProviderRequestType.MfaLogin:
                    var email = await _userService.GetProviderValueAsync(request.User.Identity, registration.Id).ConfigureAwait(false);
                    if (email == null)
                    {
                        return null;
                    }

                    var sessionId = Guid.NewGuid().ToString("N");
                    if (!await _notification.IsSupportedAsync().ConfigureAwait(false))
                    {
                        throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("no notification service."));
                    }

                    var code = _random.Next(1000, 1000000).ToString().PadLeft(6, '0');
                    var notification = new MfaMailNotification(code, request.User, _options.CompanyName, _options.SupportEmail);
                    await _notification.SendNotificationAsync(notification).ConfigureAwait(false);

                    return new MailRegistrationInfo(registration.Id, email, sessionId, error);
                case ProviderRequestType.MfaRegister:
                    return new RegisterMailRegistrationInfo(registration.Id, error);
                case ProviderRequestType.MfaList:
                    if (request.User.LinkedProviders.Contains(registration.Id))
                    {
                        var address = await _userService.GetProviderValueAsync(request.User.Identity, registration.Id).ConfigureAwait(false);
                        if (address == null)
                        {
                            return null;
                        }

                        return GetMfaListRegistrationInfoWithMail(request, registration, address);
                    }
                    else if (!request.User.HasMfaRegistration)
                    {
                        return GetMfaListRegistrationInfoRegister(request, registration);
                    }

                    return null;
                case ProviderRequestType.Login:
                case ProviderRequestType.Invitation:
                case ProviderRequestType.Profile:
                default:
                    return null;
            }
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRegistration configuration, object request)
        {
            if (request is RegisterMailLoginRequest registration)
            {
                if (string.IsNullOrEmpty(registration.SessionId))
                {
                    var sessionId = Guid.NewGuid().ToString("N");
                    var user = await _userService.GetUserByIdentityAsync(registration.Identity).ConfigureAwait(false);

                    var code = _random.Next(1000, 1000000).ToString().PadLeft(6, '0');
                    var notification = new MfaMailNotification(code, user, _options.CompanyName, _options.SupportEmail);

                    await _notification.SendNotificationAsync(notification).ConfigureAwait(false);

                    var response = new RegisterMailRegistrationInfo(registration.ProviderId, registration.EmailAddress, sessionId);

                    var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
                    uriBuilder.AppendPath(_options.AccountEndpoint);
                    uriBuilder.AppendPath("login/mfa");

                    if (!string.IsNullOrWhiteSpace(registration.ReturnUrl))
                    {
                        uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, registration.ReturnUrl);
                    }

                    throw new ErrorResponseException<MfaLoginResponse>(new MfaLoginResponse(response, uriBuilder.ToString(), registration.ReturnUrl));
                }
                else
                {
                    if (_linkUserService == null)
                    {
                        throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("registration not supported!"));
                    }

                    var user = await _userService.GetUserByIdentityAsync(registration.Identity).ConfigureAwait(false);

                    await _linkUserService.LinkUserAsync(user, new MailLoginData(configuration, registration.EmailAddress));
                    var mfaIdentity = GenerateMfaIdentity();

                    return new SignInResponse(mfaIdentity, registration.ReturnUrl, AuthenticationMode.Mfa);
                }
            }
            else if (request is ProcessMailLoginRequest process)
            {
                var mfaIdentity = GenerateMfaIdentity();
                return new SignInResponse(mfaIdentity, process.ReturnUrl, AuthenticationMode.Mfa);
            }

            throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("unknown"));
        }

        private ClaimsIdentity GenerateMfaIdentity()
        {
            var identity = new ClaimsIdentity(_options.MfaAuthenticationScheme);
            identity.AddClaim(new Claim(Constants.Claims.Amr, Constants.OpenId.Amr.Mfa));
            identity.AddClaim(new Claim(Constants.Claims.Amr, Constants.OpenId.Amr.Mail));
            return identity;
        }

        private ILoginRegistrationInfo GetMfaListRegistrationInfoRegister(ProviderRequest request, ILoginRegistration registration)
        {
            var description = _stringResources.GetResource(StringResource.MfaListRegisterMail);

            return GetMfaListRegistrationInfoWithDescription(request, registration, description);
        }

        private ILoginRegistrationInfo GetMfaListRegistrationInfoWithDescription(ProviderRequest request, ILoginRegistration registration, string description)
        {
            request.ProviderErrors.TryGetValue(registration.Id, out var error);

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login/mfa");
            uriBuilder.AppendPath(registration.Id);

            if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            }

            return new MfaProviderListInfo(registration.Id, uriBuilder.ToString(), description, "fa-solid fa-at", error);
        }

        private ILoginRegistrationInfo GetMfaListRegistrationInfoWithMail(ProviderRequest request, ILoginRegistration registration, string email)
        {
            var masked = MailRegistrationInfo.Mask(email);
            var description = string.Format(_stringResources.GetResource(StringResource.OneTimeCodeViaEmail), masked);

            return GetMfaListRegistrationInfoWithDescription(request, registration, description);
        }
    }
}
