using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    public class DistributedRefreshTokenCache : IRefreshTokenCache
    {
        private readonly IDistributedCache _cache;
        private readonly ISymmetricDataEncryption _dataEncryption;

        public DistributedRefreshTokenCache(IDistributedCache cache, ISymmetricDataEncryption dataEncryption)
        {
            _cache = cache;
            _dataEncryption = dataEncryption;
        }

        public Task ExtendLifetimeAsync(string key, TimeSpan extendBy)
        {
            throw new NotImplementedException();
        }

        public Task<IRefreshTokenCacheItem> GetAsync(string key)
        {
            throw new NotImplementedException();
        }

        public async Task<string> SetAsync(IdentityData data, TimeSpan validFor)
        {
            var cacheKey = Guid.NewGuid().ToString("N");

            var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(data)).ConfigureAwait(false);

            await _cache.SetStringAsync(
                cacheKey,
                encrypted.Data,
                new DistributedCacheEntryOptions()
                {
                    AbsoluteExpirationRelativeToNow = validFor,
                });

            return $"{cacheKey}.{encrypted.Key}";
        }
    }
}