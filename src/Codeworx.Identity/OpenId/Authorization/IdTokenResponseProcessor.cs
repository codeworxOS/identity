using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Internal;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.Token;
using static Codeworx.Identity.Constants;

namespace Codeworx.Identity.OpenId.Authorization
{
    public class IdTokenResponseProcessor : IAuthorizationResponseProcessor
    {
        private readonly IClientService _clientService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IDefaultSigningKeyProvider _defaultSigningKeyProvider;

        public IdTokenResponseProcessor(IClientService clientService, IEnumerable<ITokenProvider> tokenProviders, IDefaultSigningKeyProvider defaultSigningKeyProvider)
        {
            _clientService = clientService;
            _tokenProviders = tokenProviders;
            _defaultSigningKeyProvider = defaultSigningKeyProvider;
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

            this.AddAtHashClaim(responseBuilder.Response.Token, payload);

            await token.SetPayloadAsync(payload, client.TokenExpiration).ConfigureAwait(false);
            var identityToken = await token.SerializeAsync().ConfigureAwait(false);

            return responseBuilder.WithIdToken(identityToken);
        }

        private void AddAtHashClaim(string accessTokenValue, IDictionary<string, object> identityClaims)
        {
            var hashAlgorithm = _defaultSigningKeyProvider.GetHashAlgorithm();
            var accessTokenHash = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(accessTokenValue));
            var atHash = accessTokenHash.Take(accessTokenHash.Length / 2).ToArray();

            identityClaims.Add(Claims.AtHash, Base64UrlEncoding.Encode(atHash));
        }
    }
}