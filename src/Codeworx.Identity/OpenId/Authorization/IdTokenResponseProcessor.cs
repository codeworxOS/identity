using System;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Internal;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OpenId.Authorization
{
    public class IdTokenResponseProcessor : IAuthorizationResponseProcessor
    {
        private readonly IClientService _clientService;
        private readonly ITokenProviderService _tokenProviderService;
        private readonly IDefaultSigningKeyProvider _defaultSigningKeyProvider;

        public IdTokenResponseProcessor(IClientService clientService, ITokenProviderService tokenProviderService, IDefaultSigningKeyProvider defaultSigningKeyProvider)
        {
            _clientService = clientService;
            _tokenProviderService = tokenProviderService;
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

            var token = await _tokenProviderService.CreateIdentityTokenAsync(parameters.Client).ConfigureAwait(false);

            var claims = data.Claims.ToList();
            claims.Add(AssignedClaim.Create(Constants.Claims.AtHash, GetAtHashClaim(responseBuilder.Response.Token)));

            await token.SetPayloadAsync(new IdentityData(data.ClientId, data.Identifier, data.Login, claims, data.ExternalTokenKey), parameters.Client.TokenExpiration).ConfigureAwait(false);
            var identityToken = await token.SerializeAsync().ConfigureAwait(false);

            return responseBuilder.WithIdToken(identityToken);
        }

        private string GetAtHashClaim(string accessTokenValue)
        {
            var hashAlgorithm = _defaultSigningKeyProvider.GetHashAlgorithm();
            var accessTokenHash = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(accessTokenValue));
            var atHash = accessTokenHash.Take(accessTokenHash.Length / 2).ToArray();

            return Base64UrlEncoding.Encode(atHash);
        }
    }
}