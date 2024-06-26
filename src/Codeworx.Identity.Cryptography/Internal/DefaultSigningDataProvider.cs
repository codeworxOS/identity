using System;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace Codeworx.Identity.Cryptography.Internal
{
    public class DefaultSigningDataProvider : IDefaultSigningDataProvider, IDisposable
    {
        private readonly ISigningDataProvider _signingDataProvider;
        private readonly IDisposable _subscription;
        private bool _disposedValue;
        private IdentityOptions _options;

        private object _signingDataLocker = new object();
        private Task<SigningData> _signingDataTask;

        public DefaultSigningDataProvider(IOptionsMonitor<IdentityOptions> monitor, ISigningDataProvider signingDataProvider)
        {
            _subscription = monitor.OnChange(p =>
            {
                _options = p;
                lock (_signingDataLocker)
                {
                    if (_signingDataTask != null && _signingDataTask.IsCompleted)
                    {
                        _signingDataTask.Result?.Dispose();
                    }

                    _signingDataTask = null;
                }
            });
            _options = monitor.CurrentValue;
            _signingDataProvider = signingDataProvider;
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<SigningData> GetSigningDataAsync(CancellationToken token)
        {
            var data = _signingDataTask;

            if (data == null)
            {
                lock (_signingDataLocker)
                {
                    if (_signingDataTask == null)
                    {
                        data = _signingDataTask = LoadDataAsync(_options.Signing, token);
                    }
                }
            }

            return await data.ConfigureAwait(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    _subscription.Dispose();
                    lock (_signingDataLocker)
                    {
                        var task = _signingDataTask;

                        if (task != null && task.IsCompleted)
                        {
                            task.Result?.Dispose();
                        }
                    }
                }

                _disposedValue = true;
            }
        }

        private HashAlgorithm CreateHashAlgorithmFromKey(ECDsaSecurityKey key)
        {
            switch (key.KeySize)
            {
                case 256:
                    return SHA256.Create();
                case 384:
                    return SHA384.Create();
                case 521:
                    return SHA512.Create();
                default:
                    break;
            }

            throw new NotSupportedException("Unsupported signing key.");
        }

        private async Task<SigningData> LoadDataAsync(SigningOptions options, CancellationToken token)
        {
            switch (options.Source)
            {
                case KeySource.TemporaryInMemory:
                    var ecd = ECDsa.Create(ECCurve.NamedCurves.nistP384);
                    var ecdKey = new ECDsaSecurityKey(ecd);
                    var hashing = CreateHashAlgorithmFromKey(ecdKey);

                    return new SigningData(ecdKey, ecd, hashing);

                case KeySource.Store:
                    return await _signingDataProvider.GetSigningDataAsync(options.Search, token).ConfigureAwait(false);
                default:
                    throw new NotSupportedException($"The signing source {options.Source} is not supported!");
            }
        }
    }
}