using System;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration.Extensions;
using Codeworx.Identity.Model;
using Codeworx.Identity.Resources;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Configuration.Infrastructure
{
    public class ConfigurationClientService : IClientService, IDisposable
    {
        private readonly IDisposable _changeToken;
        private readonly IUserService _userService;
        private readonly IStringResources _stringResources;
        private bool _disposedValue;
        private ClientConfigOptions _options;

        public ConfigurationClientService(
            IOptionsMonitor<ClientConfigOptions> options,
            IUserService userService,
            IStringResources stringResources)
        {
            _options = options.CurrentValue;
            _changeToken = options.OnChange(p => _options = p);
            _userService = userService;
            _stringResources = stringResources;
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
                return await config.ToRegistration(_userService, _stringResources, clientIdentifier);
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