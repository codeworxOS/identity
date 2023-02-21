using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class RefreshTokenService : ITokenService<RefreshTokenRequest>
    {
        private readonly ITokenProviderService _tokenProviderService;
        private readonly IEnumerable<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>> _processors;
        private readonly IUserService _userService;
        private readonly IClientService _clientService;
        private readonly IIdentityService _identityService;

        public RefreshTokenService(
            ITokenProviderService tokenProviderService,
            IEnumerable<IIdentityRequestProcessor<IRefreshTokenParameters, RefreshTokenRequest>> processors,
            IUserService userService,
            IClientService clientService,
            IIdentityService identityService)
        {
            _tokenProviderService = tokenProviderService;
            _processors = processors;
            _userService = userService;
            _clientService = clientService;
            _identityService = identityService;
        }

        public async Task<TokenResponse> ProcessAsync(RefreshTokenRequest request, CancellationToken token = default)
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

            var accessToken = await _tokenProviderService.CreateAccessTokenAsync(parameters.Client, token).ConfigureAwait(false);
            var identityToken = await _tokenProviderService.CreateIdentityTokenAsync(parameters.Client, token).ConfigureAwait(false);

            await accessToken.SetPayloadAsync(identityData, parameters.TokenValidUntil)
                    .ConfigureAwait(false);
            await identityToken.SetPayloadAsync(identityData, parameters.TokenValidUntil)
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

            return new TokenResponse(accessTokenValue, identityTokenValue, Constants.OAuth.TokenType.Bearer, (int)parameters.Client.TokenExpiration.TotalSeconds, scope, refreshToken);
        }
    }
}