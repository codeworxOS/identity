using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
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

        public async Task<AuthenticateClientResult> AuthenticateClient(TokenRequest request, string clientId, string clientSecret)
        {
            var requestClientIdHasValue = !string.IsNullOrEmpty(request?.ClientId);
            var requestClientSecretHasValue = !string.IsNullOrEmpty(request?.ClientSecret);
            var headerClientIdHasValue = !string.IsNullOrEmpty(clientId);
            var headerClientSecretHasValue = !string.IsNullOrEmpty(clientSecret);

            var result = new AuthenticateClientResult();

            if (((requestClientSecretHasValue && headerClientSecretHasValue) ||
                (requestClientIdHasValue && headerClientIdHasValue)) && request?.ClientId != clientId)
            {
                result.TokenResult = new InvalidRequestResult();
                return result;
            }

            if (requestClientIdHasValue && requestClientSecretHasValue && !headerClientIdHasValue && !headerClientSecretHasValue)
            {
                clientId = request?.ClientId;
                clientSecret = request?.ClientSecret;
            }
            else if (!headerClientIdHasValue || !headerClientSecretHasValue)
            {
                result.TokenResult = new InvalidClientResult();
                return result;
            }

            if (string.IsNullOrEmpty(clientId) || string.IsNullOrEmpty(clientSecret))
            {
                result.TokenResult = new InvalidClientResult();
                return result;
            }

            var client = await _clientService.GetById(clientId)
                                             .ConfigureAwait(false);

            if (client == null)
            {
                result.TokenResult = new InvalidClientResult();
                return result;
            }

            var secretHash = _hashingProvider.Hash(clientSecret, client.ClientSecretSalt);
            if (!secretHash.SequenceEqual(client.ClientSecretHash))
            {
                result.TokenResult = new InvalidClientResult();
                return result;
            }

            result.ClientRegistration = client;
            return result;
        }
    }
}