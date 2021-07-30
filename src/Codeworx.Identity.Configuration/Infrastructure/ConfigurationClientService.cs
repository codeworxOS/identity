using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration.Extensions;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Model;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Configuration.Infrastructure
{
    public class ConfigurationClientService : IClientService, IDisposable
    {
        private readonly IDisposable _changeToken;
        private readonly IHashingProvider _hashing;
        private readonly IUserService _userService;
        private bool _disposedValue;
        private ClientConfigOptions _options;

        public ConfigurationClientService(IOptionsMonitor<ClientConfigOptions> options, IHashingProvider hashing, IUserService userService)
        {
            _options = options.CurrentValue;
            _changeToken = options.OnChange(p => _options = p);
            _hashing = hashing;
            _userService = userService;
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<IClientRegistration> GetById(string clientIdentifier)
        {
            if (_options.TryGetValue(clientIdentifier, out var config))
            {
                return await config.ToRegistration(_userService, _hashing, clientIdentifier);
            }

            return null;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _changeToken.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}