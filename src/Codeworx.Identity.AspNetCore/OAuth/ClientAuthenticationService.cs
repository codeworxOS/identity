using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;

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

        public async Task<IClientRegistration> AuthenticateClient(TokenRequest request)
        {
            var result = new AuthenticateClientResult();

            var client = await _clientService.GetById(request.ClientId)
                                             .ConfigureAwait(false);

            if (client == null)
            {
                ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
            }

            if (client.ClientSecretHash != null)
            {
                if (string.IsNullOrWhiteSpace(request.ClientSecret))
                {
                    ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
                }

                var secretHash = _hashingProvider.Hash(request.ClientSecret, client.ClientSecretSalt);
                if (!secretHash.SequenceEqual(client.ClientSecretHash))
                {
                    ErrorResponse.Throw(Constants.OAuth.Error.InvalidClient);
                }
            }

            return client;
        }
    }
}