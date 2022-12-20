using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Distributed;

namespace Codeworx.Identity.Cache
{
    public class DistributedMailMfaCodeCache : IMailMfaCodeCache
    {
        private static readonly Random _random;
        private readonly IDistributedCache _cache;

        static DistributedMailMfaCodeCache()
        {
            _random = new Random();
        }

        public DistributedMailMfaCodeCache(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<string> GetAsync(string key, CancellationToken token = default)
        {
            var cacheKey = $"{Constants.Cache.MailMfaPrefix}_{key}";

            var result = await _cache.GetStringAsync(cacheKey, token).ConfigureAwait(false);

            return result;
        }

        public async Task<string> CreateAsync(string key, TimeSpan validFor, CancellationToken token = default)
        {
            var cacheKey = $"{Constants.Cache.MailMfaPrefix}_{key}";

            var result = _random.Next(1000, 1000000).ToString().PadLeft(6, '0');
            await _cache.SetStringAsync(cacheKey, result, new DistributedCacheEntryOptions { AbsoluteExpirationRelativeToNow = validFor }, token).ConfigureAwait(false);

            return result;
        }
    }
}