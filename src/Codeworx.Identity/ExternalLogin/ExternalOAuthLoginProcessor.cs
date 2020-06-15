using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.ExternalLogin
{
    public class ExternalOAuthLoginProcessor : IExternalLoginProcessor
    {
        private readonly IDistributedCache _cache;
        private readonly IExternalOAuthTokenService _tokenService;
        private readonly string _redirectUri;

        public ExternalOAuthLoginProcessor(IBaseUriAccessor baseUriAccessor, IExternalOAuthTokenService tokenService, IDistributedCache cache, IOptionsSnapshot<IdentityOptions> options)
        {
            _tokenService = tokenService;
            _cache = cache;

            var redirectUirBuilder = new UriBuilder(baseUriAccessor.BaseUri.ToString());
            redirectUirBuilder.AppendPath($"{options.Value.AccountEndpoint}/oauthlogin");

            _redirectUri = redirectUirBuilder.ToString();
        }

        public Type RequestParameterType { get; } = typeof(ExternalOAuthLoginRequest);

        public Type ConfigurationType { get; } = typeof(ExternalOAuthLoginConfiguration);

        public async Task<string> GetProcessorUrlAsync(ProviderRequest request, object configuration)
        {
            var oauthConfiguration = this.ToOAuthLoginConfiguration(configuration);

            var codeUriBuilder = new UriBuilder(oauthConfiguration.BaseUri.ToString());

            codeUriBuilder.AppendPath(oauthConfiguration.AuthorizationEndpoint);

            codeUriBuilder.AppendQueryParameter(Constants.OAuth.ResponseTypeName, Constants.OAuth.ResponseType.Code);
            codeUriBuilder.AppendQueryParameter(Constants.OAuth.ClientIdName, oauthConfiguration.ClientId);
            codeUriBuilder.AppendQueryParameter(Constants.OAuth.RedirectUriName, _redirectUri);

            if (oauthConfiguration.Scope != null)
            {
                codeUriBuilder.AppendQueryParameter("scope", oauthConfiguration.Scope);
            }

            var state = Guid.NewGuid().ToString("N");

            await _cache.SetStringAsync(state, request.ReturnUrl ?? string.Empty, new DistributedCacheEntryOptions { SlidingExpiration = TimeSpan.FromMinutes(5) });

            codeUriBuilder.AppendQueryParameter(Constants.OAuth.StateName, state);

            return codeUriBuilder.ToString();
        }

        public async Task<ExternalLoginResponse> ProcessAsync(object request, object configuration)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var loginRequest = ToOAuthLoginRequest(request);
            var oauthConfiguration = this.ToOAuthLoginConfiguration(configuration);

            var returnUrl = await _cache.GetStringAsync(loginRequest.State);

            if (returnUrl != null)
            {
                await _cache.RemoveAsync(loginRequest.State);
            }
            else
            {
                throw new ErrorResponseException<InvalidStateResponse>(new InvalidStateResponse("State is invalid."));
            }

            var userId = await _tokenService.GetUserIdAsync(oauthConfiguration, loginRequest.Code, _redirectUri);

            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = null;
            }

            return new ExternalLoginResponse(userId, returnUrl);
        }

        private ExternalOAuthLoginConfiguration ToOAuthLoginConfiguration(object configuration)
        {
            var oauthConfiguration = configuration as ExternalOAuthLoginConfiguration;

            if (oauthConfiguration == null)
            {
                throw new ArgumentException($"The argument ist not of type {ConfigurationType}", nameof(configuration));
            }

            return oauthConfiguration;
        }

        private ExternalOAuthLoginRequest ToOAuthLoginRequest(object request)
        {
            var loginRequest = request as ExternalOAuthLoginRequest;

            if (loginRequest == null)
            {
                throw new ArgumentException($"The argument ist not of type {RequestParameterType}", nameof(request));
            }

            return loginRequest;
        }
    }
}