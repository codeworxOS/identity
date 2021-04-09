using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    public class DistributedExternalTokenCache : IExternalTokenCache
    {
        private readonly IDistributedCache _cache;

        public DistributedExternalTokenCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public virtual async Task<ExternalTokenData> GetAsync(string key, TimeSpan extend)
        {
            var entry = await _cache.GetStringAsync(key)
                                         .ConfigureAwait(false);

            if (entry == null)
            {
                throw new CacheEntryNotFoundException();
            }

            await _cache.SetStringAsync(
              key,
              entry,
              new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = extend })
                .ConfigureAwait(false);

            return JsonConvert.DeserializeObject<ExternalTokenData>(entry);
        }

        public virtual async Task<string> SetAsync(ExternalTokenData value, TimeSpan validFor)
        {
            var key = Guid.NewGuid().ToString("N");

            await _cache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(value),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = validFor })
            .ConfigureAwait(false);

            return key;
        }

        public virtual async Task UpdateAsync(string key, ExternalTokenData value, TimeSpan validFor)
        {
            await _cache.SetStringAsync(
                key,
                JsonConvert.SerializeObject(value),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = validFor })
            .ConfigureAwait(false);
        }
    }
}