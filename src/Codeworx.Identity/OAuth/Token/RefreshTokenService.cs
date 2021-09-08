using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenService : ITokenService<RefreshTokenRequest>
    {
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IEnumerable<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>> _processors;
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        private readonly IIdentityService _identityService;

        public RefreshTokenService(
            IEnumerable<ITokenProvider> tokenProviders,
            IEnumerable<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>> processors,
            IUserService userService,
            IClientService clientService,
            IIdentityService identityService)
        {
            _tokenProviders = tokenProviders;
            _processors = processors;
            _userService = userService;
            _clientService = clientService;
            _identityService = identityService;
        }

        public async Task<TokenResponse> ProcessAsync(RefreshTokenRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var builder = new RefreshTokenParametersBuilder();

            foreach (var processor in _processors.OrderBy(p => p.SortOrder))
            {
                await processor.ProcessAsync(builder, request).ConfigureAwait(false);
            }

            var parameters = builder.Parameters;

            var identityData = await _identityService.GetIdentityAsync(parameters).ConfigureAwait(false);

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == Constants.Token.Jwt);

            var client = await _clientService.GetById(parameters.ClientId).ConfigureAwait(false);

            var accessToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);
            var identityToken = await tokenProvider.CreateAsync(null).ConfigureAwait(false);

            await accessToken.SetPayloadAsync(identityData.GetTokenClaims(ClaimTarget.AccessToken), client.TokenExpiration)
                    .ConfigureAwait(false);
            await identityToken.SetPayloadAsync(identityData.GetTokenClaims(ClaimTarget.IdToken), client.TokenExpiration)
                    .ConfigureAwait(false);

            string refreshToken = null;

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            string scope = null;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);

                if (scopeClaim.Values.Contains(Constants.OpenId.Scopes.OfflineAccess))
                {
                    refreshToken = parameters.RefreshToken;
                }
            }

            var accessTokenValue = await accessToken.SerializeAsync().ConfigureAwait(false);
            var identityTokenValue = await identityToken.SerializeAsync().ConfigureAwait(false);

            return new TokenResponse(accessTokenValue, identityTokenValue, Constants.OAuth.TokenType.Bearer, (int)client.TokenExpiration.TotalSeconds, scope, refreshToken);
        }
    }
}