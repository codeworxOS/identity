using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AccessTokenResponseProcessor : IAuthorizationResponseProcessor
    {
        private readonly IClientService _clientService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public AccessTokenResponseProcessor(IClientService clientService, IEnumerable<ITokenProvider> tokenProviders, IBaseUriAccessor baseUriAccessor = null)
        {
            _clientService = clientService;
            _tokenProviders = tokenProviders;
            _baseUriAccessor = baseUriAccessor;
        }

        public async Task<IAuthorizationResponseBuilder> ProcessAsync(IAuthorizationParameters parameters, IdentityData data, IAuthorizationResponseBuilder responseBuilder)
        {
            parameters = parameters ?? throw new ArgumentNullException(nameof(parameters));
            data = data ?? throw new ArgumentNullException(nameof(data));
            responseBuilder = responseBuilder ?? throw new ArgumentNullException(nameof(responseBuilder));

            if (!parameters.ResponseTypes.Contains(Constants.OAuth.ResponseType.Token))
            {
                return responseBuilder;
            }

            var provider = _tokenProviders.First(p => p.TokenType == "jwt");
            var token = await provider.CreateAsync(null);

            var client = parameters.Client;
            var payload = data.GetTokenClaims(ClaimTarget.AccessToken);
            var issuer = _baseUriAccessor?.BaseUri.OriginalString;

            await token.SetPayloadAsync(payload, client.TokenExpiration).ConfigureAwait(false);

            var accessToken = await token.SerializeAsync().ConfigureAwait(false);

            return responseBuilder.WithAccessToken(accessToken, client.TokenExpiration);
        }
    }
}