using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AccessTokenResponseProcessor : IAuthorizationResponseProcessor
    {
        private readonly IClientService _clientService;
        private readonly ITokenProviderService _tokenProviderService;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public AccessTokenResponseProcessor(IClientService clientService, ITokenProviderService tokenProviderService, IBaseUriAccessor baseUriAccessor = null)
        {
            _clientService = clientService;
            _tokenProviderService = tokenProviderService;
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

            var token = await _tokenProviderService.CreateAccessTokenAsync(parameters.Client);
            await token.SetPayloadAsync(data, parameters.Client.TokenExpiration).ConfigureAwait(false);

            var accessToken = await token.SerializeAsync().ConfigureAwait(false);

            return responseBuilder.WithAccessToken(accessToken, parameters.Client.TokenExpiration);
        }
    }
}