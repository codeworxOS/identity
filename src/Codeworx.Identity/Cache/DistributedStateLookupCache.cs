using System;
using System.Threading.Tasks;
using Codeworx.Identity.Login.OAuth;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    public class DistributedStateLookupCache : IStateLookupCache
    {
        private readonly IDistributedCache _cache;

        public DistributedStateLookupCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<StateLookupItem> GetAsync(string state)
        {
            var lookup = await _cache.GetStringAsync(state)
                                                     .ConfigureAwait(false);

            if (lookup == null)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<StateLookupItem>(lookup);
        }

        public async Task SetAsync(string state, StateLookupItem value, TimeSpan validFor)
        {
            await _cache.SetStringAsync(
                state,
                JsonConvert.SerializeObject(value),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = validFor })
            .ConfigureAwait(false);
        }
    }
}