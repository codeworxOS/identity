using System;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class LocalCertificateStore : ICertificateStore, IDisposable
    {
        private readonly IDisposable _subscription;
        private bool _disposedValue;
        private SigningOptions _options;

        public LocalCertificateStore(IOptionsMonitor<IdentityOptions> monitor)
        {
            _options = monitor.CurrentValue.Signing;
            _subscription = monitor.OnChange(p => _options = p.Signing);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public Task<X509Certificate2> LoadAsync(string key, CancellationToken token)
        {
            return Task.Run(() =>
            {
                var store = new X509Store(_options.Name, _options.Location);
                store.Open(OpenFlags.ReadOnly);

                var searchResult = store.Certificates.Find(_options.FindBy, key, false);

                var now = DateTime.Now;

                var cert = searchResult.OfType<X509Certificate2>()
                                        .Where(p => p.NotBefore <= now && p.NotAfter > now)
                                        .OrderByDescending(p => p.NotAfter)
                                        .FirstOrDefault();

                if (cert == null)
                {
                    throw new CertificateNotFoundException(key);
                }

                return cert;
            });
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                }

                _disposedValue = true;
            }
        }
    }
}
