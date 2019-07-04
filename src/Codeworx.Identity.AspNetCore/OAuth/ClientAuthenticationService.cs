using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class ClientAuthenticationService : IClientAuthenticationService
    {
        private readonly IClientService _clientService;
        private readonly IHashingProvider _hashingProvider;

        public ClientAuthenticationService(IClientService clientService, IHashingProvider hashingProvider)
        {
            _clientService = clientService;
            _hashingProvider = hashingProvider;
        }

        public async Task<(ITokenResult TokenResult, IClientRegistration ClientRegistration)> AuthenticateClient(TokenRequest request, (string ClientId, string ClientSecret)? authorizationHeader)
        {
            var (requestClientIdHasValue, requestClientSecretHasValue, headerClientIdHasValue, headerClientSecretHasValue) = (!string.IsNullOrEmpty(request?.ClientId),
                                                                                                                           !string.IsNullOrEmpty(request?.ClientSecret),
                                                                                                                           !string.IsNullOrEmpty(authorizationHeader?.ClientId),
                                                                                                                           !string.IsNullOrEmpty(authorizationHeader?.ClientSecret));

            if (requestClientSecretHasValue && headerClientSecretHasValue
                || requestClientIdHasValue && headerClientIdHasValue && request?.ClientId != authorizationHeader?.ClientId)
            {
                return (new InvalidRequestResult(), null);
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
                return (new InvalidClientResult(), null);
            }

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                return (new InvalidClientResult(), null);
            }

            var client = await _clientService.GetById(clientId)
                                             .ConfigureAwait(false);

            if (client == null)
            {
                return (new InvalidClientResult(), null);
            }

            var secretHash = _hashingProvider.Hash(clientSecret, client.ClientSecretSalt);
            if (!secretHash.SequenceEqual(client.ClientSecretHash))
            {
                return (new InvalidClientResult(), null);
            }

            return (null, client);
        }
    }
}
