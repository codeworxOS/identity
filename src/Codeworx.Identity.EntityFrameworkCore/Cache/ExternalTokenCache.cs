using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    public class ExternalTokenCache<TContext> : EncryptedCache<TContext, ExternalTokenData>, IExternalTokenCache
        where TContext : DbContext
    {
        public ExternalTokenCache(TContext context, ISymmetricDataEncryption dataEncryption, ILogger<ExternalTokenCache<TContext>> logger)
            : base(context, logger, dataEncryption)
        {
        }

        public async Task ExtendAsync(string key, TimeSpan extension, CancellationToken token = default)
        {
            var validUntil = DateTime.UtcNow.Add(extension);
            await ExtendEntryAsync(CacheType.ExternalTokenData, key, validUntil, token);
        }

        public virtual async Task<ExternalTokenData> GetAsync(string key, CancellationToken token = default)
        {
            var entry = await GetEntryAsync(CacheType.ExternalTokenData, key, false, token).ConfigureAwait(false);

            return entry.Data;
        }

        public virtual async Task<string> SetAsync(ExternalTokenData value, DateTimeOffset validUntil, CancellationToken token = default)
        {
            var cacheKey = Guid.NewGuid().ToString("N");
            return await AddEntryAsync(CacheType.ExternalTokenData, cacheKey, value, validUntil, token).ConfigureAwait(false);
        }

        public virtual async Task UpdateAsync(string key, ExternalTokenData value, CancellationToken token = default)
        {
            await UpdateEntryAsync(CacheType.ExternalTokenData, key, value, token).ConfigureAwait(false);
        }

        protected override Guid? GetUserId(ExternalTokenData data)
        {
            return null;
        }
    }
}
