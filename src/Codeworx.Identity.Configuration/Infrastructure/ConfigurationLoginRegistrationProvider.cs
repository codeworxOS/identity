using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration.Model;
using Codeworx.Identity.Login;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.Configuration.Infrastructure
{
    public class ConfigurationLoginRegistrationProvider : ILoginRegistrationProvider, IDisposable
    {
        private readonly IDisposable _reloadToken;
        private readonly IConfigurationSection _section;
        private readonly IServiceProvider _serviceProvider;
        private readonly IDictionary<string, IProcessorTypeLookup> _processorTypeLookups;
        private bool _disposedValue;
        private List<ILoginRegistration> _registrations;

        public ConfigurationLoginRegistrationProvider(IConfigurationSection section, IEnumerable<IProcessorTypeLookup> processorTypeLookups, IServiceProvider serviceProvider)
        {
            _section = section;
            _serviceProvider = serviceProvider;
            _processorTypeLookups = processorTypeLookups.ToDictionary(p => p.Key, p => p);
            _reloadToken = _section.GetReloadToken().RegisterChangeCallback(ReloadData, null);

            ReloadData(null);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(LoginProviderType loginProviderType, string userName = null)
        {
            return Task.FromResult<IEnumerable<ILoginRegistration>>(_registrations);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _reloadToken.Dispose();
                }

                _disposedValue = true;
            }
        }

        private void ReloadData(object state)
        {
            var registrations = new List<ILoginRegistration>();

            foreach (var client in _section.GetChildren())
            {
                var data = new SectionData();
                var id = client.Key;
                client.Bind(data);

                if (_processorTypeLookups.TryGetValue(data.Type, out var processorType))
                {
                    object config = null;

                    if (processorType.ConfigurationType != null)
                    {
                        config = Activator.CreateInstance(processorType.ConfigurationType);
                        client.GetSection(Constants.LoginRegistrationProcessorConfigSectionName).Bind(config);
                    }

                    var registration = new LoginRegistration(processorType.Type, id, data.Name, config);

                    registrations.Add(registration);
                }
            }

            _registrations = registrations;
        }

        private class SectionData
        {
            public SectionData()
            {
            }

            public string Name { get; set; }

            public string Type { get; set; }
        }
    }
}
