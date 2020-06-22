using System.Threading.Tasks;
using Codeworx.Identity.Configuration.Extensions;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Configuration.Infrastructure
{
    public class ConfigurationClientService : IClientService
    {
        private readonly IHashingProvider _hashing;
        private readonly IUserService _userService;
        private ClientConfigOptions _options;

        public ConfigurationClientService(IOptionsSnapshot<ClientConfigOptions> options, IHashingProvider hashing, IUserService userService)
        {
            _options = options.Value;
            _hashing = hashing;
            _userService = userService;
        }

        public async Task<IClientRegistration> GetById(string clientIdentifier)
        {
            if (_options.TryGetValue(clientIdentifier, out var config))
            {
                return await config.ToRegistration(_userService, _hashing, clientIdentifier);
            }

            return null;
        }
    }
}