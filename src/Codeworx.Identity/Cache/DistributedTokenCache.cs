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

        public async Task ExtendLifetimeAsync(TokenType tokenType, string key, DateTimeOffset validUntil, CancellationToken token = default)
        {
            GetKeys(key, out var cacheKey, out var encryptionKey);
            TokenCacheEntry entry = await GetTokenCacheEntryAsync(tokenType, cacheKey, token).ConfigureAwait(false);
            entry.ValidUntil = validUntil;

            await _cache.SetStringAsync(
               $"identity_token_{tokenType}_{cacheKey}",
               JsonConvert.SerializeObject(entry),
               new DistributedCacheEntryOptions() { AbsoluteExpiration = entry.ValidUntil, },
               token);
        }

        public async Task<ITokenCacheItem> GetAsync(TokenType tokenType, string key, CancellationToken token = default)
        {
            GetKeys(key, out var cacheKey, out var encryptionKey);
            TokenCacheEntry entry = await GetTokenCacheEntryAsync(tokenType, cacheKey, token).ConfigureAwait(false);

            var data = await _dataEncryption.DecryptAsync(entry.Data, encryptionKey);

            var identityData = JsonConvert.DeserializeObject<IdentityData>(data);

            return new TokenCacheItem(identityData, entry.ValidUntil);
        }

        public async Task<string> SetAsync(TokenType tokenType, IdentityData data, DateTimeOffset validUntil, CancellationToken token = default)
        {
            var cacheKey = Guid.NewGuid().ToString("N");

            var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            var entry = new TokenCacheEntry { Data = encrypted.Data, ValidUntil = validUntil };

            await _cache.SetStringAsync(
                $"identity_token_{tokenType}_{cacheKey}",
                JsonConvert.SerializeObject(entry),
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

        private async Task<TokenCacheEntry> GetTokenCacheEntryAsync(TokenType tokenType, string cacheKey, CancellationToken token)
        {
            var cachedGrantInformation = await _cache.GetStringAsync($"identity_token_{tokenType}_{cacheKey}", token).ConfigureAwait(false);
            if (cachedGrantInformation == null)
            {
                throw new CacheEntryNotFoundException();
            }

            var entry = JsonConvert.DeserializeObject<TokenCacheEntry>(cachedGrantInformation);
            return entry;
        }

        private class TokenCacheEntry
        {
            public TokenCacheEntry()
            {
            }

            public string Data { get; set; }

            public DateTimeOffset ValidUntil { get; set; }
        }

        private class TokenCacheItem : ITokenCacheItem
        {
            public TokenCacheItem(IdentityData identityData, DateTimeOffset validUntil)
            {
                IdentityData = identityData;
                ValidUntil = validUntil;
            }

            public IdentityData IdentityData { get; }

            public DateTimeOffset ValidUntil { get; }
        }
    }
}