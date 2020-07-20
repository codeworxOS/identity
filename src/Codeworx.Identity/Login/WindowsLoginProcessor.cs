using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login
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
            uriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);

            var result = new RedirectRegistrationInfo(configuration.Id, configuration.Name, uriBuilder.ToString());

            return Task.FromResult<ILoginRegistrationInfo>(result);
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRequest request, object configuration)
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

            var sid = loginRequest.WindowsIdentity.FindFirst(ClaimTypes.PrimarySid).Value;

            var identity = await _identityService.LoginExternalAsync(request.ProviderId, sid).ConfigureAwait(false);

            return new SignInResponse(identity, loginRequest.ReturnUrl);
        }
    }
}