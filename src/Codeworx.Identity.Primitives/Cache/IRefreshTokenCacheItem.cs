using System;

namespace Codeworx.Identity.Cache
{
    public interface IRefreshTokenCacheItem
    {
        IdentityData IdentityData { get; }

        DateTime ValidUntil { get; }
    }
}
