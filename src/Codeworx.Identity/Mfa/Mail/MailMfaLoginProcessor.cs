using System;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Mfa;
using Codeworx.Identity.Mail;
using Codeworx.Identity.Model;
using Codeworx.Identity.Notification;
using Codeworx.Identity.Resources;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaLoginProcessor : ILoginProcessor
    {
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IMailMfaCodeCache _codeCache;
        private readonly ILinkUserService _linkUserService;
        private readonly IMailConnector _mailConnector;
        private readonly INotificationProcessor _notificationProcessor;
        private readonly IdentityOptions _options;
        private readonly IStringResources _stringResources;
        private readonly IUserService _userService;

        public MailMfaLoginProcessor(
            IUserService userService,
            IStringResources stringResources,
            IBaseUriAccessor baseUriAccessor,
            IOptionsSnapshot<IdentityOptions> options,
            IMailMfaCodeCache codeCache,
            ILinkUserService linkUserService = null,
            IMailConnector mailConnector = null,
            INotificationProcessor notificationProcessor = null)
        {
            _options = options.Value;
            _userService = userService;
            _stringResources = stringResources;
            _baseUriAccessor = baseUriAccessor;
            _codeCache = codeCache;
            _linkUserService = linkUserService;
            _mailConnector = mailConnector;
            _notificationProcessor = notificationProcessor;
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

                    if (!request.HeaderOnly)
                    {
                        await EnsureCodeNotificationAsync(request.User, request.UserSession, email).ConfigureAwait(false);
                    }

                    return new MailRegistrationInfo(registration.Id, email, request.UserSession, error);
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
                    else if (request.User.HasMfaRegistration && !request.IsMfaAuthenticated)
                    {
                        return null;
                    }

                    return GetMfaListRegistrationInfoRegister(request, registration);

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
                    var user = await _userService.GetUserByIdentityAsync(registration.Identity).ConfigureAwait(false);

                    var sessionId = Guid.NewGuid().ToString("N");

                    await EnsureCodeNotificationAsync(user, sessionId, registration.EmailAddress).ConfigureAwait(false);

                    MfaLoginResponse registerResponse = CreateRegisterMailResponse(registration, sessionId);

                    throw new ErrorResponseException<MfaLoginResponse>(registerResponse);
                }
                else
                {
                    if (_linkUserService == null)
                    {
                        throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("registration not supported!"));
                    }

                    var user = await _userService.GetUserByIdentityAsync(registration.Identity).ConfigureAwait(false);

                    var cachedCode = await _codeCache.GetAsync(registration.SessionId).ConfigureAwait(false);
                    if (cachedCode == null)
                    {
                        await EnsureCodeNotificationAsync(user, registration.SessionId, registration.EmailAddress).ConfigureAwait(false);
                        throw new ErrorResponseException<MfaLoginResponse>(CreateRegisterMailResponse(registration, _stringResources.GetResource(StringResource.InvalidOneTimeCode)));
                    }

                    if (cachedCode != registration.Code)
                    {
                        throw new ErrorResponseException<MfaLoginResponse>(CreateRegisterMailResponse(registration, _stringResources.GetResource(StringResource.InvalidOneTimeCode)));
                    }

                    await _linkUserService.LinkUserAsync(user, new MailLoginData(configuration, registration.EmailAddress));
                    var mfaIdentity = GenerateMfaIdentity();

                    return new SignInResponse(mfaIdentity, registration.ReturnUrl, AuthenticationMode.Mfa);
                }
            }
            else if (request is ProcessMailLoginRequest process)
            {
                var cachedCode = await _codeCache.GetAsync(process.SessionId).ConfigureAwait(false);

                if (cachedCode != process.OneTimeCode)
                {
                    var response = await CreateProcessMailResponseAsync(process, configuration, _stringResources.GetResource(StringResource.InvalidOneTimeCode)).ConfigureAwait(false);

                    throw new ErrorResponseException<MfaLoginResponse>(response);
                }

                var mfaIdentity = GenerateMfaIdentity();
                return new SignInResponse(mfaIdentity, process.ReturnUrl, AuthenticationMode.Mfa);
            }

            throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("unknown"));
        }

        private async Task<MfaLoginResponse> CreateProcessMailResponseAsync(ProcessMailLoginRequest process, ILoginRegistration registration, string error = null)
        {
            var user = await _userService.GetUserByIdentityAsync(process.Identity).ConfigureAwait(false);
            var providerRequest = new ProviderRequest(ProviderRequestType.MfaLogin, false, process.ReturnUrl, user: user, userSession: process.SessionId);

            if (error != null)
            {
                providerRequest.ProviderErrors.Add(registration.Id, error);
            }

            var info = await GetRegistrationInfoAsync(providerRequest, registration).ConfigureAwait(false);

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login/mfa");

            if (!string.IsNullOrWhiteSpace(process.ReturnUrl))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, process.ReturnUrl);
            }

            var registerResponse = new MfaLoginResponse(info, uriBuilder.ToString(), process.ReturnUrl);
            return registerResponse;
        }

        private MfaLoginResponse CreateRegisterMailResponse(RegisterMailLoginRequest registration, string error = null)
        {
            var response = new RegisterMailRegistrationInfo(registration.ProviderId, registration.EmailAddress, registration.SessionId, error);

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri);
            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("login/mfa");

            if (!string.IsNullOrWhiteSpace(registration.ReturnUrl))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, registration.ReturnUrl);
            }

            var registerResponse = new MfaLoginResponse(response, uriBuilder.ToString(), registration.ReturnUrl);
            return registerResponse;
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

        private async Task EnsureCodeNotificationAsync(IUser user, string sessionId, string email, CancellationToken token = default)
        {
            if (_mailConnector == null || _notificationProcessor == null)
            {
                throw new ErrorResponseException<NotAcceptableResponse>(new NotAcceptableResponse("no notification service."));
            }

            var code = await _codeCache.GetAsync(sessionId, token).ConfigureAwait(false);

            if (code == null)
            {
                code = await _codeCache.CreateAsync(sessionId, TimeSpan.FromMinutes(15), token).ConfigureAwait(false);

                var notification = new MfaMailNotification(code, user, _options.CompanyName, _options.SupportEmail);
                var content = await _notificationProcessor.GetNotificationContentAsync(notification).ConfigureAwait(false);

                await _mailConnector.SendAsync(new System.Net.Mail.MailAddress(email), notification.Subject, content).ConfigureAwait(false);
            }
        }
    }
}
