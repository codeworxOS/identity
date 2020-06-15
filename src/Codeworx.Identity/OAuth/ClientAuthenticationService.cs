using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth
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

        public async Task<IClientRegistration> AuthenticateClient(string clientId, string clientSecret)
        {
            var client = await _clientService.GetById(clientId)
                                             .ConfigureAwait(false);

            if (client == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
            }

            if (client.ClientSecretHash != null)
            {
                if (string.IsNullOrWhiteSpace(clientSecret))
                {
                    ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
                }

                var secretHash = _hashingProvider.Hash(clientSecret, client.ClientSecretSalt);
                if (!secretHash.SequenceEqual(client.ClientSecretHash))
                {
                    ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
                }
            }

            return client;
        }
    }
}