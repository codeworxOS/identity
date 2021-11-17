using System;
using System.Threading;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IRefreshTokenCache
    {
        Task<IRefreshTokenCacheItem> GetAsync(string key, CancellationToken token = default);

        Task ExtendLifetimeAsync(string key, TimeSpan extendBy, CancellationToken token = default);

        Task<string> SetAsync(IdentityData data, TimeSpan validFor, CancellationToken token = default);
    }
}
