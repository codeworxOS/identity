using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenExchangeService : ITokenService<TokenExchangeRequest>
    {
        private readonly ITokenProviderService _tokenProviderService;
        private readonly IEnumerable<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>> _processors;
        private readonly IIdentityService _identityService;

        public TokenExchangeService(
            ITokenProviderService tokenProviderService,
            IEnumerable<IIdentityRequestProcessor<ITokenExchangeParameters, TokenExchangeRequest>> processors,
            IIdentityService identityService)
        {
            _tokenProviderService = tokenProviderService;
            _processors = processors;
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

            string accessTokenValue = null;
            string identityTokenValue = null;

            if (!parameters.RequestedTokenTypes.Any() || parameters.RequestedTokenTypes.Contains(Constants.TokenExchange.TokenType.AccessToken))
            {
                var accessToken = await _tokenProviderService.CreateAccessTokenAsync(parameters.Client, token).ConfigureAwait(false);

                await accessToken.SetPayloadAsync(identityData, parameters.Client.TokenExpiration)
                        .ConfigureAwait(false);

                accessTokenValue = await accessToken.SerializeAsync().ConfigureAwait(false);
            }

            if (parameters.RequestedTokenTypes.Contains(Constants.TokenExchange.TokenType.IdToken))
            {
                var identityToken = await _tokenProviderService.CreateTokenAsync(Constants.Token.Jwt, TokenType.IdToken, null, token).ConfigureAwait(false);
                await identityToken.SetPayloadAsync(identityData, parameters.Client.TokenExpiration)
                        .ConfigureAwait(false);

                identityTokenValue = await identityToken.SerializeAsync().ConfigureAwait(false);
            }

            string refreshTokenValue = null;

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            string scope = null;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);

                if (scopeClaim.Values.Contains(Constants.OpenId.Scopes.OfflineAccess))
                {
                    var refreshToken = await _tokenProviderService.CreateRefreshTokenAsync(token).ConfigureAwait(false);
                    await refreshToken.SetPayloadAsync(identityData, TimeSpan.FromDays(30 * 6), token).ConfigureAwait(false);

                    refreshTokenValue = await refreshToken.SerializeAsync(token).ConfigureAwait(false);
                }
            }

            return new TokenResponse(accessTokenValue, identityTokenValue, Constants.OAuth.TokenType.Bearer, (int)parameters.Client.TokenExpiration.TotalSeconds, scope, refreshTokenValue);
        }
    }
}