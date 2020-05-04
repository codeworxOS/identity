using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenService : ITokenService
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IRequestValidator<TokenRequest> _requestValidator;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IEnumerable<ITokenResultService> _tokenResultServices;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IClientService _clientService;

        public TokenService(
            IAuthorizationCodeCache cache,
            IEnumerable<ITokenResultService> tokenResultServices,
            IRequestValidator<TokenRequest> requestValidator,
            IClientAuthenticationService clientAuthenticationService,
            IClientService clientService,
            IEnumerable<ITokenProvider> tokenProviders)
        {
            _tokenProviders = tokenProviders;
            _tokenResultServices = tokenResultServices;
            _requestValidator = requestValidator;
            _clientAuthenticationService = clientAuthenticationService;
            _clientService = clientService;
            _cache = cache;
        }

        public async Task<TokenResponse> AuthorizeRequest(
            TokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _requestValidator.ValidateAsync(request)
                                                          .ConfigureAwait(false);

            var tokenResultService = _tokenResultServices.FirstOrDefault(p => p.SupportedGrantType == request.GrantType);
            if (tokenResultService == null)
            {
                throw new ErrorResponseException<ErrorResponse>(new ErrorResponse(Constants.OAuth.Error.UnsupportedGrantType, string.Empty, string.Empty));
            }

            var client = await _clientAuthenticationService.AuthenticateClient(request)
                                                                                 .ConfigureAwait(false);

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            var authorizationCodeTokenRequest = request as AuthorizationCodeTokenRequest;

            if (request == null || authorizationCodeTokenRequest == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var deserializedGrantInformation = await _cache.GetAsync(authorizationCodeTokenRequest.Code)
                .ConfigureAwait(false);

            if (deserializedGrantInformation == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            // todo reimplement
            ////var clientId = deserializedGrantInformation[Constants.OAuth.ClientIdName];
            ////if (clientId != request.ClientId)
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            ////}

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == "jwt");
            var token = await tokenProvider.CreateAsync(null).ConfigureAwait(false);

            var payload = deserializedGrantInformation.GetTokenClaims(ClaimTarget.AccessToken);
            ////var issuer = _baseUriAccessor?.BaseUri.OriginalString;
            ////var audience = cacheData[Constants.OAuth.ClientIdName];
            ////cacheData.TryGetValue(Constants.OAuth.ScopeName, out var scope);
            ////cacheData.TryGetValue(Constants.OAuth.NonceName, out var nonce);

            await token.SetPayloadAsync(payload, "issuer", "audience", deserializedGrantInformation.ToClaimsIdentity(), "scope", "nonce", TimeSpan.FromHours(1));

            var accessToken = await token.SerializeAsync().ConfigureAwait(false);

            token = await tokenProvider.CreateAsync(null).ConfigureAwait(false);
            payload = deserializedGrantInformation.GetTokenClaims(ClaimTarget.IdToken);
            await token.SetPayloadAsync(payload, "issuer", "audience", deserializedGrantInformation.ToClaimsIdentity(), "scope", "nonce", TimeSpan.FromHours(1));

            var identityToken = await token.SerializeAsync().ConfigureAwait(false);

            ////var accessToken = await tokenResultService.CreateAccessToken(deserializedGrantInformation, client.TokenExpiration);
            ////var identityToken = await tokenResultService.CreateIdToken(deserializedGrantInformation, client.TokenExpiration);

            ////deserializedGrantInformation.TryGetValue(Constants.OAuth.ScopeName, out var scope);

            return new TokenResponse(accessToken, identityToken, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, "scope");
        }
    }
}