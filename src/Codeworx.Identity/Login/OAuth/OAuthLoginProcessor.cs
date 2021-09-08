using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Login.OAuth
{
    // EventIds 145xx
    public class OAuthLoginProcessor : ILoginProcessor
    {
        private static readonly Action<ILogger, string, Exception> _processingLoginMessage;
        private static readonly Action<ILogger, string, Exception> _processingParaemtersNullMessage;
        private static readonly Action<ILogger, Exception> _stateNotFoundMessage;
        private static readonly Action<ILogger, Exception> _externalTokenCacheMissingMessage;
        private readonly IIdentityService _identityService;
        private readonly ILogger<OAuthLoginProcessor> _logger;
        private readonly IExternalTokenCache _externalTokenCache;
        private readonly IdentityOptions _identityOptions;
        private readonly IExternalOAuthTokenService _tokenService;
        private readonly IStateLookupCache _stateCache;
        private readonly string _baseUri;

        static OAuthLoginProcessor()
        {
            _processingLoginMessage = LoggerMessage.Define<string>(LogLevel.Information, new EventId(14501), "Processing Login for registration Id: {registrationId}.");
            _processingParaemtersNullMessage = LoggerMessage.Define<string>(LogLevel.Error, new EventId(14502), "The request cannot be processed, becuase the parameter {parameterName} is null.");
            _stateNotFoundMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14503), "Unable to get the matching state for the callback!");
            _externalTokenCacheMissingMessage = LoggerMessage.Define(LogLevel.Error, new EventId(14504), "External Token cach is not registered!");
        }

        public OAuthLoginProcessor(
            IBaseUriAccessor baseUriAccessor,
            IExternalOAuthTokenService tokenService,
            IOptionsSnapshot<IdentityOptions> options,
            IStateLookupCache stateCache,
            IIdentityService identityService,
            ILogger<OAuthLoginProcessor> logger,
            IExternalTokenCache externalTokenCache = null)
        {
            _tokenService = tokenService;
            _stateCache = stateCache;
            _identityService = identityService;
            _logger = logger;
            _externalTokenCache = externalTokenCache;
            _identityOptions = options.Value;
            _baseUri = baseUriAccessor.BaseUri.ToString();
        }

        public Type RequestParameterType { get; } = typeof(OAuthLoginRequest);

        public Task<ILoginRegistrationInfo> GetRegistrationInfoAsync(ProviderRequest request, ILoginRegistration configuration)
        {
            var oauthConfiguration = this.ToOAuthLoginConfiguration(configuration.ProcessorConfiguration);
            var cssClass = oauthConfiguration.CssClass;

            var redirectUriBuilder = new UriBuilder(_baseUri);
            redirectUriBuilder.AppendPath(_identityOptions.AccountEndpoint);

            var returnUrl = request.ReturnUrl ?? $"{_identityOptions.AccountEndpoint}/me";

            switch (request.Type)
            {
                case ProviderRequestType.Login:
                    redirectUriBuilder.AppendPath("oauth");
                    redirectUriBuilder.AppendPath(configuration.Id);
                    redirectUriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
                    break;
                case ProviderRequestType.Invitation:
                    redirectUriBuilder.AppendPath("oauth");
                    redirectUriBuilder.AppendPath(configuration.Id);
                    redirectUriBuilder.AppendQueryParameter(Constants.ReturnUrlParameter, returnUrl);
                    redirectUriBuilder.AppendQueryParameter(Constants.InvitationParameter, request.InvitationCode);
                    break;
                case ProviderRequestType.Profile:
                    redirectUriBuilder.AppendPath("me");
                    redirectUriBuilder.AppendPath(configuration.Id);
                    break;
            }

            if (!string.IsNullOrEmpty(request.Prompt))
            {
                redirectUriBuilder.AppendQueryParameter(Constants.OAuth.PromptName, request.Prompt);
            }

            string error = null;
            request.ProviderErrors.TryGetValue(configuration.Id, out error);

            ILoginRegistrationInfo result = null;

            if (request.Type == ProviderRequestType.Profile)
            {
                var isLinked = request.User.LinkedProviders.Contains(configuration.Id);
                redirectUriBuilder.AppendPath(isLinked ? "unlink" : "link");

                result = new RedirectProfileRegistrationInfo(configuration.Id, configuration.Name, cssClass, redirectUriBuilder.ToString(), isLinked, error);
            }
            else
            {
                result = new RedirectRegistrationInfo(configuration.Id, configuration.Name, cssClass, redirectUriBuilder.ToString(), error);
            }

            return Task.FromResult<ILoginRegistrationInfo>(result);
        }

        public async Task<SignInResponse> ProcessAsync(ILoginRegistration registration, object request)
        {
            if (registration is null)
            {
                var ex = new ArgumentNullException(nameof(registration));
                _processingParaemtersNullMessage(_logger, nameof(registration), ex);
                throw ex;
            }

            if (request == null)
            {
                var ex = new ArgumentNullException(nameof(request));
                _processingParaemtersNullMessage(_logger, nameof(request), ex);
                throw ex;
            }

            _processingLoginMessage(_logger, registration.Id, null);

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
                var ex = new ErrorResponseException<InvalidStateResponse>(new InvalidStateResponse("State is invalid."));
                _stateNotFoundMessage(_logger, ex);
                throw ex;
            }

            var loginData = new OAuthLoginData(registration, externalIdentity, oauthConfiguration, stateItem.InvitationCode);
            var identity = await _identityService.LoginExternalAsync(loginData).ConfigureAwait(false);

            if (oauthConfiguration.TokenHandling != ExternalTokenHandling.None)
            {
                var access_token = externalIdentity.FindFirst(Constants.OAuth.AccessTokenName)?.Value;
                var id_token = externalIdentity.FindFirst(Constants.OpenId.IdTokenName)?.Value;
                var refresh_token = externalIdentity.FindFirst(Constants.OAuth.RefreshTokenName)?.Value;

                var data = new ExternalTokenData
                {
                    AccessToken = access_token,
                    IdToken = id_token,
                    RefreshToken = refresh_token,
                    RegistrationId = registration.Id,
                };

                if (_externalTokenCache == null)
                {
                    var ex = new MissingDependencyException(typeof(IExternalTokenCache));
                    _externalTokenCacheMissingMessage(_logger, ex);
                    throw ex;
                }

                var code = await _externalTokenCache.SetAsync(data, _identityOptions.CookieExpiration).ConfigureAwait(false);

                identity.AddClaim(new System.Security.Claims.Claim(Constants.Claims.ExternalTokenKey, code));
            }

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