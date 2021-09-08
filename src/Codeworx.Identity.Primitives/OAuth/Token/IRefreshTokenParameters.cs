using Codeworx.Identity.Cache;

namespace Codeworx.Identity.OAuth.Token
{
    public interface IRefreshTokenParameters : IIdentityDataParameters
    {
        string RefreshToken { get; }

        string ClientSecret { get; }

        IRefreshTokenCacheItem CacheItem { get; }
    }
}
