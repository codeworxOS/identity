using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Token;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    // EventIds 150xx
    public class DistributedTokenCache : ITokenCache
    {
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;

        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;

        private readonly IDistributedCache _cache;

        private readonly ISymmetricDataEncryption _dataEncryption;

        private readonly ILogger<DistributedTokenCache> _logger;

        static DistributedTokenCache()
        {
            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(15003), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(
                LogLevel.Trace,
                new EventId(15004),
                "The format of cache key {Key} is invalid!");
        }

        public DistributedTokenCache(
            IDistributedCache cache,
            ISymmetricDataEncryption dataEncryption,
            ILogger<DistributedTokenCache> logger)
        {
            _cache = cache;
            _dataEncryption = dataEncryption;
            _logger = logger;
        }

        public Task ExtendLifetimeAsync(TokenType tokenType, string key, TimeSpan extendBy, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<ITokenCacheItem> GetAsync(TokenType tokenType, string key, CancellationToken token = default)
        {
            GetKeys(key, out var cacheKey, out var encryptionKey);
            var cachedGrantInformation = await _cache.GetStringAsync($"identity_token_{tokenType}_{cacheKey}", token).ConfigureAwait(false);
            if (cachedGrantInformation == null)
            {
                throw new CacheEntryNotFoundException();
            }

            var data = await _dataEncryption.DecryptAsync(cachedGrantInformation, encryptionKey);

            var identityData = JsonConvert.DeserializeObject<IdentityData>(data);

            return new TokenCacheItem(identityData, DateTime.UtcNow.Add(TimeSpan.FromHours(1)));
        }

        public async Task<string> SetAsync(TokenType tokenType, IdentityData data, DateTime validUntil, CancellationToken token = default)
        {
            var cacheKey = Guid.NewGuid().ToString("N");

            var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            await _cache.SetStringAsync(
                $"identity_token_{tokenType}_{cacheKey}",
                encrypted.Data,
                new DistributedCacheEntryOptions() { AbsoluteExpiration = validUntil, },
                token);

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

        private class TokenCacheItem : ITokenCacheItem
        {
            public TokenCacheItem(IdentityData identityData, DateTime validUntil)
            {
                IdentityData = identityData;
                ValidUntil = validUntil;
            }

            public IdentityData IdentityData { get; }

            public DateTime ValidUntil { get; }
        }
    }
}