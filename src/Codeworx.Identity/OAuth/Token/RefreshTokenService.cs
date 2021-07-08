using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenService : ITokenService<RefreshTokenRequest>
    {
        private readonly IRequestValidator<RefreshTokenRequest> _validator;
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly IRefreshTokenCache _refreshTokenCache;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public RefreshTokenService(
            IRequestValidator<RefreshTokenRequest> validator,
            IClientAuthenticationService clientAuthenticationService,
            IRefreshTokenCache refreshTokenCache,
            IEnumerable<ITokenProvider> tokenProviders,
            IUserService userService,
            IIdentityService identityService)
        {
            _validator = validator;
            _clientAuthenticationService = clientAuthenticationService;
            _refreshTokenCache = refreshTokenCache;
            _tokenProviders = tokenProviders;
            _userService = userService;
            _identityService = identityService;
        }

        public async Task<TokenResponse> ProcessAsync(RefreshTokenRequest request)
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

            var cacheEntry = await _refreshTokenCache.GetAsync(request.RefreshToken).ConfigureAwait(false);

            if (cacheEntry == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            if (cacheEntry.IdentityData.ClientId != request.ClientId)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidGrant);
            }

            // TODO extend refresh_token lifetime or recreate;
            var clientId = cacheEntry.IdentityData.ClientId;
            var user = await _userService.GetUserByIdAsync(cacheEntry.IdentityData.Identifier).ConfigureAwait(false);
            var identity = await _identityService.GetClaimsIdentityFromUserAsync(user).ConfigureAwait(false);

            if (!string.IsNullOrWhiteSpace(cacheEntry.IdentityData.ExternalTokenKey))
            {
                identity.AddClaim(new System.Security.Claims.Claim(Constants.Claims.ExternalTokenKey, cacheEntry.IdentityData.ExternalTokenKey));
            }

            var scopeClaim = cacheEntry.IdentityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);
            var scopes = scopeClaim?.Values?.ToArray() ?? new string[] { };

            // TODO chekc if requested Scopes match previous;
            var parameters = new RefreshTokenParameters(clientId, scopes, identity);

            var identityData = await _identityService.GetIdentityAsync(parameters).ConfigureAwait(false);

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == Constants.Token.Jwt);

            var accessToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);
            var identityToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);

            await accessToken.SetPayloadAsync(cacheEntry.IdentityData.GetTokenClaims(ClaimTarget.AccessToken), client.TokenExpiration)
                    .ConfigureAwait(false);
            await identityToken.SetPayloadAsync(cacheEntry.IdentityData.GetTokenClaims(ClaimTarget.IdToken), client.TokenExpiration)
                    .ConfigureAwait(false);

            string refreshToken = null;
            string scope = null;

            if (scopes.Any())
            {
                scope = string.Join(" ", scopeClaim.Values);

                if (scopes.Contains(Constants.OpenId.Scopes.OfflineAccess))
                {
                    refreshToken = request.RefreshToken;
                }
            }

            var accessTokenValue = await accessToken.SerializeAsync().ConfigureAwait(false);
            var identityTokenValue = await identityToken.SerializeAsync().ConfigureAwait(false);

            return new TokenResponse(accessTokenValue, identityTokenValue, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, scope, refreshToken);
        }
    }
}