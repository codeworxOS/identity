using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    public class ClientCredentialsTokenService : ITokenService<ClientCredentialsTokenRequest>
    {
        private readonly IReadOnlyList<IIdentityRequestProcessor<IClientCredentialsParameters, ClientCredentialsTokenRequest>> _processors;
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly ITokenProviderService _tokenProviderService;
        private readonly IIdentityService _identityService;

        public ClientCredentialsTokenService(
            IEnumerable<IIdentityRequestProcessor<IClientCredentialsParameters, ClientCredentialsTokenRequest>> processors,
            IClientAuthenticationService clientAuthenticationService,
            ITokenProviderService tokenProviderService,
            IIdentityService identityService)
        {
            _processors = processors.OrderBy(p => p.SortOrder).ToImmutableList();
            _clientAuthenticationService = clientAuthenticationService;
            _tokenProviderService = tokenProviderService;
            _identityService = identityService;
        }

        public async Task<TokenResponse> ProcessAsync(ClientCredentialsTokenRequest request, CancellationToken token = default)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var builder = new ClientCredentialsParametersBuilder();

            foreach (var item in _processors)
            {
                await item.ProcessAsync(builder, request);
            }

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.GrantType)))
            ////{
            ////    ErrorResponse.Throw(Constants.OAuth.Error.UnauthorizedClient);
            ////}

            var identityParameters = builder.Parameters;

            var accessToken = await _tokenProviderService.CreateAccessTokenAsync(identityParameters.Client, token).ConfigureAwait(false);

            var identityData = await _identityService.GetIdentityAsync(identityParameters).ConfigureAwait(false);

            await accessToken.SetPayloadAsync(identityData, identityParameters.Client.TokenExpiration, token)
                    .ConfigureAwait(false);

            var scopeClaim = identityData.Claims.FirstOrDefault(p => p.Type.First() == Constants.OAuth.ScopeName);

            string scope = null;

            if (scopeClaim != null)
            {
                scope = string.Join(" ", scopeClaim.Values);
            }

            var accessTokenValue = await accessToken.SerializeAsync(token).ConfigureAwait(false);

            return new TokenResponse(accessTokenValue, null, Constants.OAuth.TokenType.Bearer, (int)identityParameters.Client.TokenExpiration.TotalSeconds, scope);
        }
    }
}