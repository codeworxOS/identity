using System;
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

        public Type ConfigurationType => null;

        public string Template => Constants.Templates.Redirect;

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration configuration)
        {
            var uriBuilder = new UriBuilder(_baseUriAccessor.BaseUri.ToString());
            uriBuilder.AppendPath($"{_options.AccountEndpoint}/winlogin");

            var returnUrl = request.ReturnUrl ?? $"{_options.AccountEndpoint}/login";

            uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);

            var result = new RedirectRegistrationInfo(configuration.Id, configuration.Name, uriBuilder.ToString());

            return Task.FromResult<ILoginRegistrationInfo>(result);
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRequest request, ILoginRegistration registration)
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

            var loginData = new WindowsLoginData(registration, loginRequest.WindowsIdentity, loginRequest.ReturnUrl);

            var identity = await _identityService.LoginExternalAsync(loginData).ConfigureAwait(false);

            return new SignInResponse(identity, loginRequest.ReturnUrl);
        }
    }
}