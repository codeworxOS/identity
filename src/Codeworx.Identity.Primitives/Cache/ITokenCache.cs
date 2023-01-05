using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.Cache
{
    public interface ITokenCache
    {
        Task<ITokenCacheItem> GetAsync(TokenType tokenType, string key, CancellationToken token = default);

        Task ExtendLifetimeAsync(TokenType tokenType, string key, TimeSpan validFor, CancellationToken token = default);

        Task<string> SetAsync(TokenType tokenType, IdentityData data, DateTime validUntil, CancellationToken token = default);
    }
}
