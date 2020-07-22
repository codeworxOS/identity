using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Codeworx.Identity.Cache
{
    public class DistributedStateLookupCache : IStateLookupCache
    {
        private readonly IDistributedCache _cache;

        public DistributedStateLookupCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> GetAsync(string state)
        {
            var lookup = await _cache.GetStringAsync(state)
                                                     .ConfigureAwait(false);

            if (lookup == null)
            {
                return null;
            }

            await _cache.RemoveAsync(state);

            return lookup;
        }

        public async Task SetAsync(string state, string value)
        {
            await _cache.SetStringAsync(
                state,
                value,
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5) })
            .ConfigureAwait(false);
        }
    }
}