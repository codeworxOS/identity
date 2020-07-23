using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthLoginProcessor : ILoginProcessor
    {
        private readonly IIdentityService _identityService;
        private readonly IdentityOptions _identityOptions;
        private readonly IExternalOAuthTokenService _tokenService;
        private readonly IStateLookupCache _stateCache;
        private readonly string _baseUri;

        public OAuthLoginProcessor(
            IBaseUriAccessor baseUriAccessor,
            IExternalOAuthTokenService tokenService,
            IOptionsSnapshot<IdentityOptions> options,
            IStateLookupCache stateCache,
            IIdentityService identityService)
        {
            _tokenService = tokenService;
            _stateCache = stateCache;
            _identityService = identityService;
            _identityOptions = options.Value;

            _baseUri = baseUriAccessor.BaseUri.ToString();
        }

        public Type RequestParameterType { get; } = typeof(OAuthLoginRequest);

        public Type ConfigurationType { get; } = typeof(OAuthLoginConfiguration);

        public string Template => Constants.Templates.Redirect;

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration configuration)
        {
            var oauthConfiguration = this.ToOAuthLoginConfiguration(configuration.ProcessorConfiguration);

            var redirectUriBuilder = new UriBuilder(_baseUri);
            redirectUriBuilder.AppendPath(_identityOptions.AccountEndpoint);
            redirectUriBuilder.AppendPath("oauth");
            redirectUriBuilder.AppendPath(configuration.Id);
            redirectUriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, request.ReturnUrl);
            if (!string.IsNullOrEmpty(request.Prompt))
            {
                redirectUriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, request.Prompt);
            }

            var result = new RedirectRegistrationInfo(configuration.Id, configuration.Name, redirectUriBuilder.ToString());

            return Task.FromResult<ILoginRegistrationInfo>(result);
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRequest request, ILoginRegistration registration)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var loginRequest = ToOAuthLoginRequest(request);
            var oauthConfiguration = this.ToOAuthLoginConfiguration(registration.ProcessorConfiguration);

            var returnUrl = await _stateCache.GetAsync(loginRequest.State);

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = null;
            }

            if (returnUrl == null)
            {
                throw new ErrorResponseException<InvalidStateResponse>(new InvalidStateResponse("State is invalid."));
            }

            var callbackUriBuilder = new UriBuilder(_baseUri);
            callbackUriBuilder.AppendPath(_identityOptions.AccountEndpoint);
            callbackUriBuilder.AppendPath("callback");
            callbackUriBuilder.AppendPath(registration.Id);

            var userId = await _tokenService.GetUserIdAsync(oauthConfiguration, loginRequest.Code, callbackUriBuilder.ToString());

            var identity = await _identityService.LoginExternalAsync(registration.Id, userId).ConfigureAwait(false);

            return new SignInResponse(identity, returnUrl);
        }

        private OAuthLoginConfiguration ToOAuthLoginConfiguration(object configuration)
        {
            var oauthConfiguration = configuration as OAuthLoginConfiguration;

            if (oauthConfiguration == null)
            {
                throw new ArgumentException($"The argument ist not of type {ConfigurationType}", nameof(configuration));
            }

            return oauthConfiguration;
        }

        private OAuthLoginRequest ToOAuthLoginRequest(object request)
        {
            var loginRequest = request as OAuthLoginRequest;

            if (loginRequest == null)
            {
                throw new ArgumentException($"The argument ist not of type {RequestParameterType}", nameof(request));
            }

            return loginRequest;
        }
    }
}