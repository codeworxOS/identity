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

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration configuration)
        {
            var oauthConfiguration = this.ToOAuthLoginConfiguration(configuration.ProcessorConfiguration);

            var redirectUriBuilder = new UriBuilder(_baseUri);
            redirectUriBuilder.AppendPath(_identityOptions.AccountEndpoint);
            redirectUriBuilder.AppendPath("oauth");
            redirectUriBuilder.AppendPath(configuration.Id);

            if (request.Type == ProviderRequestType.Invitation)
            {
                redirectUriBuilder.AppendQueryParameter(Constants.InvitationParameter, request.InvitationCode);
            }
            else
            {
                var returnUrl = request.ReturnUrl ?? $"{_identityOptions.AccountEndpoint}/me";
                redirectUriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
            }

            if (!string.IsNullOrEmpty(request.Prompt))
            {
                redirectUriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, request.Prompt);
            }

            string error = null;
            request.ProviderErrors.TryGetValue(configuration.Id, out error);

            var result = new RedirectRegistrationInfo(configuration.Id, configuration.Name, redirectUriBuilder.ToString(), error);

            return Task.FromResult<ILoginRegistrationInfo>(result);
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRegistration registration, object request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var loginRequest = ToOAuthLoginRequest(request);
            var oauthConfiguration = this.ToOAuthLoginConfiguration(registration.ProcessorConfiguration);

            var callbackUriBuilder = new UriBuilder(_baseUri);
            callbackUriBuilder.AppendPath(_identityOptions.AccountEndpoint);
            callbackUriBuilder.AppendPath("callback");
            callbackUriBuilder.AppendPath(registration.Id);

            var externalIdentity = await _tokenService.GetIdentityAsync(oauthConfiguration, loginRequest.Code, callbackUriBuilder.ToString());

            var cacheKey = loginRequest.State;
            StateLookupItem stateItem = null;

            if (oauthConfiguration.RedirectCacheMethod == RedirectCacheMethod.UseNonce)
            {
                cacheKey = externalIdentity.FindFirst(Constants.OAuth.NonceName)?.Value;
            }

            if (cacheKey != null)
            {
                stateItem = await _stateCache.GetAsync(cacheKey);
            }

            if (stateItem == null)
            {
                throw new ErrorResponseException<InvalidStateResponse>(new InvalidStateResponse("State is invalid."));
            }

            var loginData = new OAuthLoginData(registration, externalIdentity, oauthConfiguration, stateItem.InvitationCode);

            var identity = await _identityService.LoginExternalAsync(loginData).ConfigureAwait(false);

            return new SignInResponse(identity, stateItem.ReturnUrl);
        }

        private OAuthLoginConfiguration ToOAuthLoginConfiguration(object configuration)
        {
            var oauthConfiguration = configuration as OAuthLoginConfiguration;

            if (oauthConfiguration == null)
            {
                throw new ArgumentException($"The argument ist not of type OAuthLoginConfiguration", nameof(configuration));
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