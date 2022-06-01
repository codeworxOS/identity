using Codeworx.Identity.Cache;

namespace Codeworx.Identity.OAuth.Token
{
    public interface IRefreshTokenParameters : IIdentityDataParameters
    {
        string RefreshToken { get; }

        IRefreshTokenCacheItem CacheItem { get; }
    }
}
