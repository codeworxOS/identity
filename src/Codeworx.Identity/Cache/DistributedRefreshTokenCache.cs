using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    // EventIds 150xx
    public class DistributedRefreshTokenCache : IRefreshTokenCache
    {
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;

        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;

        private readonly IDistributedCache _cache;

        private readonly ISymmetricDataEncryption _dataEncryption;

        private readonly ILogger<DistributedRefreshTokenCache> _logger;

        static DistributedRefreshTokenCache()
        {
            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(15003), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(
                LogLevel.Trace,
                new EventId(15004),
                "The format of cache key {Key} is invalid!");
        }

        public DistributedRefreshTokenCache(
            IDistributedCache cache,
            ISymmetricDataEncryption dataEncryption,
            ILogger<DistributedRefreshTokenCache> logger)
        {
            _cache = cache;
            _dataEncryption = dataEncryption;
            _logger = logger;
        }

        public Task ExtendLifetimeAsync(string key, TimeSpan extendBy)
        {
            throw new NotImplementedException();
        }

        public async Task<IRefreshTokenCacheItem> GetAsync(string key)
        {
            GetKeys(key, out var cacheKey, out var encryptionKey);
            var cachedGrantInformation = await _cache.GetStringAsync(cacheKey).ConfigureAwait(false);
            if (cachedGrantInformation == null)
            {
                return null;
            }

            var data = await _dataEncryption.DecryptAsync(cachedGrantInformation, encryptionKey);

            var identityData = JsonConvert.DeserializeObject<IdentityData>(data);

            return new RefreshTokenCacheItem(identityData);
        }

        public async Task<string> SetAsync(IdentityData data, TimeSpan validFor)
        {
            var cacheKey = Guid.NewGuid().ToString("N");

            var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            await _cache.SetStringAsync(
                cacheKey,
                encrypted.Data,
                new DistributedCacheEntryOptions() { AbsoluteExpirationRelativeToNow = validFor, });

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

        private class RefreshTokenCacheItem : IRefreshTokenCacheItem
        {
            public RefreshTokenCacheItem(IdentityData identityData)
            {
                IdentityData = identityData;
            }

            public IdentityData IdentityData { get; }

            public DateTime ValidUntil => throw new NotImplementedException();
        }
    }
}