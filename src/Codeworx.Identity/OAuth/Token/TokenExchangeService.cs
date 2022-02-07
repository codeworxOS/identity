using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenExchangeService : ITokenService<TokenExchangeRequest>
    {
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IEnumerable<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>> _processors;
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        private readonly IRefreshTokenCache _refreshTokenCache;
        private readonly IIdentityService _identityService;

        public TokenExchangeService(
            IEnumerable<ITokenProvider> tokenProviders,
            IEnumerable<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>> processors,
            IUserService userService,
            IClientService clientService,
            IRefreshTokenCache refreshTokenCache,
            IIdentityService identityService)
        {
            _tokenProviders = tokenProviders;
            _processors = processors;
            _userService = userService;
            _clientService = clientService;
            _refreshTokenCache = refreshTokenCache;
            _identityService = identityService;
        }

        public async Task<TokenResponse> ProcessAsync(TokenExchangeRequest request, CancellationToken token = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var builder = new TokenExchangeParametersBuilder();

            foreach (var processor in _processors.OrderBy(p => p.SortOrder))
            {
                await processor.ProcessAsync(builder, request).ConfigureAwait(false);
            }

            var parameters = builder.Parameters;

            var identityData = await _identityService.GetIdentityAsync(parameters).ConfigureAwait(false);

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == Constants.Token.Jwt);

            string accessTokenValue = null;
            string identityTokenValue = null;

            if (!parameters.RequestedTokenTypes.Any() || parameters.RequestedTokenTypes.Contains(Constants.TokenExchange.TokenType.AccessToken))
            {
                var accessToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);

                await accessToken.SetPayloadAsync(identityData.GetTokenClaims(ClaimTarget.AccessToken), parameters.Client.TokenExpiration)
                        .ConfigureAwait(false);

                accessTokenValue = await accessToken.SerializeAsync().ConfigureAwait(false);
            }

            if (parameters.RequestedTokenTypes.Contains(Constants.TokenExchange.TokenType.IdToken))
            {
                var identityToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);

                await identityToken.SetPayloadAsync(identityData.GetTokenClaims(ClaimTarget.IdToken), parameters.Client.TokenExpiration)
                        .ConfigureAwait(false);

                identityTokenValue = await identityToken.SerializeAsync().ConfigureAwait(false);
            }

            string refreshToken = null;

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            string scope = null;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);

                if (scopeClaim.Values.Contains(Constants.OpenId.Scopes.OfflineAccess))
                {
                    refreshToken = await _refreshTokenCache.SetAsync(identityData, TimeSpan.FromDays(30 * 6));
                }
            }

            return new TokenResponse(accessTokenValue, identityTokenValue, Constants.OAuth.TokenType.Bearer, (int)parameters.Client.TokenExpiration.TotalSeconds, scope, refreshToken);
        }
    }
}