using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Token;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    // EventIds 150xx
    public class TokenCache<TContext> : EncryptedCache<TContext, IdentityData>, ITokenCache
        where TContext : DbContext
    {
        public TokenCache(
            TContext context,
            ISymmetricDataEncryption dataEncryption,
            ILogger<TokenCache<TContext>> logger)
            : base(context, logger, dataEncryption)
        {
        }

        public async Task ExtendLifetimeAsync(TokenType tokenType, string key, TimeSpan validFor, CancellationToken token = default)
        {
            var validUntil = DateTime.UtcNow.Add(validFor);
            await ExtendEntryAsync(GetCacheType(tokenType), key, validUntil, token).ConfigureAwait(false);
        }

        public async Task<ITokenCacheItem> GetAsync(TokenType tokenType, string key, CancellationToken token = default)
        {
            var entry = await GetEntryAsync(GetCacheType(tokenType), key, false, token);
            return new TokenCacheItem(entry.Data, entry.ValidUntil);
        }

        public async Task<string> SetAsync(TokenType tokenType, IdentityData data, DateTime validUntil, CancellationToken token = default)
        {
            var cacheKey = Guid.NewGuid().ToString("N");
            return await AddEntryAsync(GetCacheType(tokenType), cacheKey, data, validUntil, token);
        }

        protected override Guid? GetUserId(IdentityData data)
        {
            return Guid.Parse(data.Identifier);
        }

        private CacheType GetCacheType(TokenType tokenType)
        {
            switch (tokenType)
            {
                case TokenType.AccessToken:
                    return CacheType.AccessToken;
                case TokenType.RefreshToken:
                    return CacheType.RefreshToken;
                case TokenType.IdToken:
                default:
                    throw new NotSupportedException("This should not happen!");
            }
        }

        private class TokenCacheItem : ITokenCacheItem
        {
            public TokenCacheItem(IdentityData identityData, DateTime validUntil)
            {
                IdentityData = identityData;
                ValidUntil = validUntil;
            }

            public IdentityData IdentityData { get; }

            public DateTime ValidUntil { get; }
        }
    }
}
