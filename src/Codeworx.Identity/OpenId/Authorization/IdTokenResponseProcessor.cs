using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OpenId.Authorization
{
    public class IdTokenResponseProcessor : IAuthorizationResponseProcessor
    {
        private readonly IClientService _clientService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;

        public IdTokenResponseProcessor(IClientService clientService, IEnumerable<ITokenProvider> tokenProviders)
        {
            _clientService = clientService;
            _tokenProviders = tokenProviders;
        }

        public async Task<IAuthorizationResponseBuilder> ProcessAsync(IAuthorizationParameters parameters, IdentityData data, IAuthorizationResponseBuilder responseBuilder)
        {
            parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            data = data ?? throw new ArgumentNullException(nameof(data));
            responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));

            if (!parameters.ResponseTypes.Contains(Constants.OpenId.ResponseType.IdToken))
            {
                return responseBuilder;
            }

            var provider = _tokenProviders.First(p => p.TokenType == "jwt");
            var token = await provider.CreateAsync(null);

            var client = await _clientService.GetById(parameters.ClientId);
            var payload = data.GetTokenClaims(ClaimTarget.IdToken);

            await token.SetPayloadAsync(payload, client.TokenExpiration).ConfigureAwait(false);

            var identityToken = await token.SerializeAsync().ConfigureAwait(false);

            return responseBuilder.WithIdToken(identityToken);
        }
    }
}