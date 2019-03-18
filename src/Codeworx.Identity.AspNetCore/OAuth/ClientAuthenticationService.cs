using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class ClientAuthenticationService : IClientAuthenticationService
    {
        private readonly IOAuthClientService _clientService;
        private readonly IHashingProvider _hashingProvider;

        public ClientAuthenticationService(IOAuthClientService clientService, IHashingProvider hashingProvider)
        {
            _clientService = clientService;
            _hashingProvider = hashingProvider;
        }

        public async Task<ITokenResult> AuthenticateClient(TokenRequest request, (string ClientId, string ClientSecret)? authorizationHeader)
        {
            var (requestClientIdHasValue, requestClientSecretHasValue, headerClientIdHasValue, headerClientSecretHasValue) = (!string.IsNullOrEmpty(request?.ClientId),
                                                                                                                           !string.IsNullOrEmpty(request?.ClientSecret),
                                                                                                                           !string.IsNullOrEmpty(authorizationHeader?.ClientId),
                                                                                                                           !string.IsNullOrEmpty(authorizationHeader?.ClientSecret));

            if (requestClientSecretHasValue && headerClientSecretHasValue)
            {
                return new InvalidRequestResult();
            }

            string clientId;
            string clientSecret;

            if (requestClientIdHasValue && requestClientSecretHasValue && !headerClientIdHasValue && !headerClientSecretHasValue)
            {
                clientId = request?.ClientId;
                clientSecret = request?.ClientSecret;
            }
            else if (!requestClientSecretHasValue && headerClientIdHasValue && headerClientSecretHasValue)
            {
                clientId = authorizationHeader?.ClientId;
                clientSecret = authorizationHeader?.ClientSecret;
            }
            else
            {
                return new InvalidClientResult();
            }

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                return new InvalidClientResult();
            }

            var client = await _clientService.GetById(clientId)
                                             .ConfigureAwait(false);

            if (client == null)
            {
                return new InvalidClientResult();
            }

            var secretHash = _hashingProvider.Hash(clientSecret, client.ClientSecretSalt);
            if (!secretHash.SequenceEqual(client.ClientSecretHash))
            {
                return new InvalidClientResult();
            }

            return null;
        }
    }
}
