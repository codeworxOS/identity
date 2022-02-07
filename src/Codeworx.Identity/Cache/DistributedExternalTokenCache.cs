using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Login;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    // EventIds 148xx
    public class DistributedExternalTokenCache : IExternalTokenCache
    {
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;
        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;

        private readonly IDistributedCache _cache;
        private readonly ILogger<DistributedExternalTokenCache> _logger;
        private readonly ISymmetricDataEncryption _dataEncryption;

        static DistributedExternalTokenCache()
        {
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14803), "The external token data {Key} was not found!");
            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(14804), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(14804), "The format of cache key {Key} is invalid!");
        }

        public DistributedExternalTokenCache(IDistributedCache cache, ILogger<DistributedExternalTokenCache> logger, ISymmetricDataEncryption dataEncryption)
        {
            _cache = cache;
            _logger = logger;
            _dataEncryption = dataEncryption;
        }

        public async Task ExtendAsync(string key, TimeSpan extension, CancellationToken token = default)
        {
            string cacheKey, encryptionKey;
            GetKeys(key, out cacheKey, out encryptionKey);

            var entry = await _cache.GetStringAsync(cacheKey, token)
                                         .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, cacheKey, null);

                throw new CacheEntryNotFoundException();
            }

            await _cache.SetStringAsync(
             cacheKey,
             entry,
             new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = extension },
             token)
               .ConfigureAwait(false);
        }

        public virtual async Task<ExternalTokenData> GetAsync(string key, CancellationToken token = default)
        {
            string cacheKey, encryptionKey;
            GetKeys(key, out cacheKey, out encryptionKey);

            var entry = await _cache.GetStringAsync(cacheKey, token)
                                         .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, cacheKey, null);

                throw new CacheEntryNotFoundException();
            }

            var decoded = await _dataEncryption.DecryptAsync(entry, encryptionKey);

            return JsonConvert.DeserializeObject<ExternalTokenData>(decoded);
        }

        public virtual async Task<string> SetAsync(ExternalTokenData value, TimeSpan validFor, CancellationToken token = default)
        {
            var cacheKey = Guid.NewGuid().ToString("N");

            var serializedValue = JsonConvert.SerializeObject(value);
            var encryped = await _dataEncryption.EncryptAsync(serializedValue);

            await _cache.SetStringAsync(
                cacheKey,
                encryped.Data,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = validFor },
                token)
            .ConfigureAwait(false);

            return $"{cacheKey}.{encryped.Key}";
        }

        public virtual async Task UpdateAsync(string key, ExternalTokenData value, CancellationToken token = default)
        {
            string cacheKey, encryptionKey;
            GetKeys(key, out cacheKey, out encryptionKey);

            var entry = await _cache.GetStringAsync(cacheKey, token)
                             .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, cacheKey, null);

                throw new CacheEntryNotFoundException();
            }

            var encoded = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(value), encryptionKey);

            await _cache.SetStringAsync(
                cacheKey,
                encoded.Data,
                token)
            .ConfigureAwait(false);
        }

        private void GetKeys(string key, out string cacheKey, out string encryptionKey)
        {
            var splitKey = key.Split('.');
            if (splitKey.Length != 2)
            {
                var exception = new InvalidCacheKeyFormatException();
                _logInvalidKeyFormat(_logger, exception);
                _logInvalidKeyFormatTrace(_logger, key, exception);
            }

            cacheKey = splitKey[0];
            encryptionKey = splitKey[1];
        }
    }
}