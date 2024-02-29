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
        private readonly IDefaultSigningDataProvider _defaultSigningDataProvider;

        public IdTokenResponseProcessor(IClientService clientService, ITokenProviderService tokenProviderService, IDefaultSigningDataProvider defaultSigningDataProvider)
        {
            _clientService = clientService;
            _tokenProviderService = tokenProviderService;
            _defaultSigningDataProvider = defaultSigningDataProvider;
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
            var hash = await GetAtHashClaim(responseBuilder.Response.Token);

            claims.Add(AssignedClaim.Create(Constants.Claims.AtHash, hash, ClaimTarget.IdToken));

            await token.SetPayloadAsync(new IdentityData(data.ClientId, data.Identifier, data.Login, claims, data.ExternalTokenKey), parameters.TokenValidUntil).ConfigureAwait(false);
            var identityToken = await token.SerializeAsync().ConfigureAwait(false);

            return responseBuilder.WithIdToken(identityToken);
        }

        private async Task<string> GetAtHashClaim(string accessTokenValue)
        {
            var data = await _defaultSigningDataProvider.GetSigningDataAsync(default);

            var hashAlgorithm = data.Hash;
            var accessTokenHash = hashAlgorithm.ComputeHash(Encoding.ASCII.GetBytes(accessTokenValue));
            var atHash = accessTokenHash.Take(accessTokenHash.Length / 2).ToArray();

            return Base64UrlEncoding.Encode(atHash);
        }
    }
}