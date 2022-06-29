using System;
using System.Threading.Tasks;
using Codeworx.Identity.Account;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Invitation;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login
{
    public class FormsLoginProcessor : ILoginProcessor
    {
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IForgotPasswordService _forgotPasswordService;
        private readonly bool _hasChangePasswordService;
        private readonly IIdentityService _identityService;
        private readonly IdentityOptions _options;

        public FormsLoginProcessor(
            IIdentityService identityService,
            IBaseUriAccessor baseUriAccessor,
            IOptionsSnapshot<IdentityOptions> options,
            IForgotPasswordService forgotPasswordService,
            IChangePasswordService changePasswordService = null)
        {
            _identityService = identityService;
            _baseUriAccessor = baseUriAccessor;
            _forgotPasswordService = forgotPasswordService;
            _hasChangePasswordService = changePasswordService != null;
            _options = options.Value;
        }

        public Type RequestParameterType { get; } = typeof(LoginFormRequest);

        public async Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration configuration)
        {
            string error = null;
            request.ProviderErrors.TryGetValue(configuration.Id, out error);

            switch (request.Type)
            {
                case ProviderRequestType.Login:
                    return new FormsLoginRegistrationInfo(configuration.Id, request.UserName, error, await GetForgotPasswordUrl(request));
                case ProviderRequestType.Invitation:
                    return new FormsInvitationRegistrationInfo(configuration.Id, request.UserName, request.Invitation.Action.HasFlag(InvitationAction.ChangeLogin), request.Invitation.Action.HasFlag(InvitationAction.ChangePassword), error);
                case ProviderRequestType.Profile:
                    var hasCurrentPassword = !string.IsNullOrEmpty(request.User.PasswordHash);
                    return new FormsProfileRegistrationInfo(configuration.Id, request.User.Name, _hasChangePasswordService, hasCurrentPassword, GetPasswodChangeUrl(request), error);
                case ProviderRequestType.MfaRegister:
                case ProviderRequestType.MfaLogin:
                default:
                    break;
            }

            throw new NotSupportedException($"Request type {request.Type} not supported!");
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRegistration registration, object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var loginRequest = ToLoginFormRequest(request);

            var returnUrl = loginRequest.ReturnUrl;

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = null;
            }

            var identity = await _identityService.LoginAsync(loginRequest.UserName, loginRequest.Password).ConfigureAwait(false);

            return new SignInResponse(identity, returnUrl, loginRequest.Remember);
        }

        private string GetPasswodChangeUrl(ProviderRequest request)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());

            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("change-password");

            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                uriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, request.Prompt);
            }

            if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            }

            return uriBuilder.ToString();
        }

        private async Task<string> GetForgotPasswordUrl(ProviderRequest request)
        {
            if (!await _forgotPasswordService.IsSupportedAsync().ConfigureAwait(false))
            {
                return null;
            }

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());

            uriBuilder.AppendPath(_options.AccountEndpoint);
            uriBuilder.AppendPath("forgot-password");

            if (!string.IsNullOrWhiteSpace(request.Prompt))
            {
                uriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, request.Prompt);
            }

            if (!string.IsNullOrWhiteSpace(request.ReturnUrl))
            {
                uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            }

            return uriBuilder.ToString();
        }

        private LoginFormRequest ToLoginFormRequest(object request)
        {
            var loginRequest = request as LoginFormRequest;

            if (loginRequest == null)
            {
                throw new ArgumentException($"The argument ist not of type {RequestParameterType}", nameof(request));
            }

            return loginRequest;
        }
    }
}