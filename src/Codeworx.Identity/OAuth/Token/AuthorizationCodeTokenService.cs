using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class AuthorizationCodeTokenService : ITokenService<AuthorizationCodeTokenRequest>
    {
        private readonly IRequestValidator<AuthorizationCodeTokenRequest> _validator;
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IRefreshTokenCache _refreshTokenCache;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IAuthorizationCodeCache _cache;

        public AuthorizationCodeTokenService(
            IAuthorizationCodeCache cache,
            IRequestValidator<AuthorizationCodeTokenRequest> validator,
            IClientAuthenticationService clientAuthenticationService,
            IRefreshTokenCache refreshTokenCache,
            IEnumerable<ITokenProvider> tokenProviders)
        {
            _validator = validator;
            _clientAuthenticationService = clientAuthenticationService;
            _refreshTokenCache = refreshTokenCache;
            this._tokenProviders = tokenProviders;
            _cache = cache;
        }

        public async Task<TokenResponse> ProcessAsync(AuthorizationCodeTokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            await _validator.ValidateAsync(request).ConfigureAwait(false);

            var client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret)
                                                                                 .ConfigureAwait(false);

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            var identityData = await _cache.GetAsync(request.Code)
                .ConfigureAwait(false);

            if (identityData == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            if (identityData.ClientId != request.ClientId)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == Constants.Token.Jwt);

            var accessToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);
            var identityToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);

            await accessToken.SetPayloadAsync(identityData.GetTokenClaims(ClaimTarget.AccessToken), client.TokenExpiration)
                    .ConfigureAwait(false);
            await identityToken.SetPayloadAsync(identityData.GetTokenClaims(ClaimTarget.IdToken), client.TokenExpiration)
                    .ConfigureAwait(false);

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            var scope = string.Empty;

            string refreshToken = null;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);

                if (scopeClaim.Values.Contains(Constants.OpenId.Scopes.OfflineAccess))
                {
                    refreshToken = await _refreshTokenCache.SetAsync(identityData, TimeSpan.FromDays(30 * 6));
                }
            }

            var accessTokenValue = await accessToken.SerializeAsync().ConfigureAwait(false);
            var identityTokenValue = await identityToken.SerializeAsync().ConfigureAwait(false);

            return new TokenResponse(accessTokenValue, identityTokenValue, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, scope, refreshToken);
        }
    }
}