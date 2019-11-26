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
        private readonly IHashingProvider _hashing;
        private readonly IDisposable _listener;
        private bool _disposedValue = false;
        private ClientConfigOptions _options;

        public ConfigurationClientService(IOptionsMonitor<ClientConfigOptions> monitor, IHashingProvider hashing)
        {
            _options = monitor.CurrentValue;
            _listener = monitor.OnChange(p => _options = p);
            _hashing = hashing;
        }

        void IDisposable.Dispose()
        {
            Dispose(true);
        }

        public Task<IClientRegistration> GetById(string clientIdentifier)
        {
            if (_options.TryGetValue(clientIdentifier, out var config))
            {
                return Task.FromResult<IClientRegistration>(config.ToRegistration(_hashing, clientIdentifier));
            }

            return Task.FromResult<IClientRegistration>(null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _listener.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}