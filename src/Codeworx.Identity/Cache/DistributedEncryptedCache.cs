using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    // EventIds 149xx
    public abstract class DistributedEncryptedCache<TData>
        where TData : class
    {
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;
        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;

        private readonly IDistributedCache _cache;
        private readonly ILogger _logger;
        private readonly ISymmetricDataEncryption _dataEncryption;

        static DistributedEncryptedCache()
        {
            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(14901), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(14901), "The format of cache key {Key} is invalid!");
        }

        public DistributedEncryptedCache(
            IDistributedCache cache,
            ILogger logger,
            ISymmetricDataEncryption dataEncryption)
        {
            _cache = cache;
            _logger = logger;
            _dataEncryption = dataEncryption;
        }

        protected abstract string CacheKeyPrefix { get; }

        protected async Task<TData> GetEntryAsync(string key, CancellationToken token = default)
        {
            string cacheKey, encryptionKey;

            GetKeys(key, out cacheKey, out encryptionKey);

            var cachedGrantInformation = await _cache.GetStringAsync($"{CacheKeyPrefix}_{cacheKey}")
                                                     .ConfigureAwait(false);

            if (cachedGrantInformation == null)
            {
                return null;
            }

            var data = await _dataEncryption.DecryptAsync(cachedGrantInformation, encryptionKey);

            await _cache.RemoveAsync(cacheKey)
                            .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<TData>(data);
        }

        protected async Task<string> AddEntryAsync(string cacheKey, TData payload, DateTimeOffset validUntil, CancellationToken token = default)
        {
            var data = JsonConvert.SerializeObject(payload);
            var encrypted = await _dataEncryption.EncryptAsync(data);

            await _cache.SetStringAsync(
                $"{CacheKeyPrefix}_{cacheKey}",
                encrypted.Data,
                new DistributedCacheEntryOptions { AbsoluteExpiration = validUntil })
            .ConfigureAwait(false);

            return $"{cacheKey}.{encrypted.Key}";
        }

        private void GetKeys(string key, out string cacheKey, out string encryptionKey)
        {
            var splitKey = key.Split('.');
            if (splitKey.Length != 2)
            {
                var exception = new InvalidCacheKeyFormatException();
                _logInvalidKeyFormat(_logger, exception);
                _logInvalidKeyFormatTrace(_logger, key, exception);

                throw exception;
            }

            cacheKey = splitKey[0];
            encryptionKey = splitKey[1];
        }
    }
}