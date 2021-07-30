using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.OAuth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    // EventIds 149xx
    public class DistributedAuthorizationCodeCache : IAuthorizationCodeCache
    {
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;
        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;

        private readonly IDistributedCache _cache;
        private readonly ILogger<DistributedAuthorizationCodeCache> _logger;
        private readonly IAuthorizationCodeGenerator _codeGenerator;
        private readonly ISymmetricDataEncryption _dataEncryption;

        static DistributedAuthorizationCodeCache()
        {
            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(14901), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(14901), "The format of cache key {Key} is invalid!");
        }

        public DistributedAuthorizationCodeCache(
            IDistributedCache cache,
            ILogger<DistributedAuthorizationCodeCache> logger,
            IAuthorizationCodeGenerator codeGenerator,
            ISymmetricDataEncryption dataEncryption)
        {
            _cache = cache;
            _logger = logger;
            _codeGenerator = codeGenerator;
            _dataEncryption = dataEncryption;
        }

        public async Task<IdentityData> GetAsync(string authorizationCode)
        {
            string cacheKey, encryptionKey;

            GetKeys(authorizationCode, out cacheKey, out encryptionKey);

            var cachedGrantInformation = await _cache.GetStringAsync(cacheKey)
                                                     .ConfigureAwait(false);

            if (cachedGrantInformation == null)
            {
                return null;
            }

            var data = await _dataEncryption.DecryptAsync(cachedGrantInformation, encryptionKey);

            var result = JsonConvert.DeserializeObject<IdentityData>(data);

            await _cache.RemoveAsync(cacheKey)
                            .ConfigureAwait(false);

            return result;
        }

        public async Task<string> SetAsync(IdentityData payload, TimeSpan validFor)
        {
            var cacheKey = await _codeGenerator.GenerateCode().ConfigureAwait(false);

            var data = JsonConvert.SerializeObject(payload);
            var encrypted = await _dataEncryption.EncryptAsync(data);

            await _cache.SetStringAsync(
                cacheKey,
                encrypted.Data,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = validFor })
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