using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.Cache
{
    public interface IRefreshTokenCache
    {
        Task<IRefreshTokenCacheItem> GetAsync(string key);

        Task ExtendLifetimeAsync(string key, TimeSpan extendBy);

        Task<string> SetAsync(IdentityData data, TimeSpan validFor);
    }
}
