using System;

namespace Codeworx.Identity.Cache
{
    public interface ITokenCacheItem
    {
        IdentityData IdentityData { get; }

        DateTimeOffset ValidUntil { get; }
    }
}
