using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    public class DistributedAuthorizationCodeCache : IAuthorizationCodeCache
    {
        private readonly IDistributedCache _cache;

        public DistributedAuthorizationCodeCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<IDictionary<string, string>> GetAsync(string authorizationCode)
        {
            var cachedGrantInformation = await _cache.GetStringAsync(authorizationCode)
                                                     .ConfigureAwait(false);

            if (cachedGrantInformation == null)
            {
                return null;
            }

            var result = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedGrantInformation);

            return result;
        }

        public async Task SetAsync(string authorizationCode, IDictionary<string, string> payload, TimeSpan timeout)
        {
            await _cache.SetStringAsync(
                authorizationCode,
                JsonConvert.SerializeObject(payload),
                new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = timeout })
            .ConfigureAwait(false);
        }
    }
}