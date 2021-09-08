using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login.Windows
{
    public class WindowsLoginProcessor : ILoginProcessor
    {
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IIdentityService _identityService;
        private readonly IdentityOptions _options;

        public WindowsLoginProcessor(IOptionsSnapshot<IdentityOptions> options, IBaseUriAccessor baseUri, IIdentityService identityService)
        {
            _options = options.Value;
            _baseUriAccessor = baseUri;
            _identityService = identityService;
        }

        public Type RequestParameterType { get; } = typeof(WindowsLoginRequest);

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration configuration)
        {
            var cssClass = Constants.Icons.Windows;

            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            var returnUrl = request.ReturnUrl ?? $"{_options.AccountEndpoint}/login";

            switch (request.Type)
            {
                case ProviderRequestType.Login:
                    uriBuilder.AppendPath($"{_options.AccountEndpoint}/winlogin/{configuration.Id}");
                    uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
                    break;
                case ProviderRequestType.Invitation:
                    uriBuilder.AppendPath($"{_options.AccountEndpoint}/winlogin/{configuration.Id}");
                    uriBuilder.AppendQueryParameter(Constants.InvitationParameter, request.InvitationCode);
                    uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
                    break;
                case ProviderRequestType.Profile:
                    uriBuilder.AppendPath($"{_options.AccountEndpoint}/me/{configuration.Id}");
                    break;
            }

            string error = null;
            request.ProviderErrors.TryGetValue(configuration.Id, out error);

            ILoginRegistrationInfo result = null;

            if (request.Type == ProviderRequestType.Profile)
            {
                var isLinked = request.User.LinkedProviders.Contains(configuration.Id);
                uriBuilder.AppendPath(isLinked ? "unlink" : "link");
                result = new RedirectProfileRegistrationInfo(configuration.Id, configuration.Name, cssClass, uriBuilder.ToString(), isLinked, error);
            }
            else
            {
                result = new RedirectRegistrationInfo(configuration.Id, configuration.Name, cssClass, uriBuilder.ToString(), error);
            }

            return Task.FromResult<ILoginRegistrationInfo>(result);
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRegistration registration, object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var loginRequest = request as WindowsLoginRequest;

            if (loginRequest == null)
            {
                throw new ArgumentException($"The argument ist not of type {RequestParameterType}", nameof(request));
            }

            var loginData = new WindowsLoginData(registration, loginRequest.WindowsIdentity, loginRequest.ReturnUrl, loginRequest.InvitationCode);

            var identity = await _identityService.LoginExternalAsync(loginData).ConfigureAwait(false);

            return new SignInResponse(identity, loginRequest.ReturnUrl);
        }
    }
}