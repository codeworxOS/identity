using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Login;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    // EventIds 147xx
    public class ExternalTokenCache<TContext> : IExternalTokenCache
        where TContext : DbContext
    {
        private static readonly Action<ILogger, string, Exception> _logKeyExpired;
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;

        private readonly TContext _context;
        private readonly ILogger<ExternalTokenCache<TContext>> _logger;

        static ExternalTokenCache()
        {
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14701), "The external token data {Key} is expired");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14703), "The external token data {Key} was not found!");
        }

        public ExternalTokenCache(TContext context, ILogger<ExternalTokenCache<TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public virtual async Task<ExternalTokenData> GetAsync(string key, TimeSpan extend)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = await cacheSet
                                .Where(p => p.CacheType == CacheType.ExternalTokenData && p.Key == key && !p.Disabled)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

                if (entry == null)
                {
                    _logKeyNotFound(_logger, key, null);
                }
                else if (entry.ValidUntil < DateTime.UtcNow)
                {
                    _logKeyExpired(_logger, key, null);
                    entry = null;
                }

                if (entry == null)
                {
                    throw new CacheEntryNotFoundException();
                }

                entry.ValidUntil = DateTime.UtcNow.Add(extend);

                await _context.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();

                return JsonConvert.DeserializeObject<ExternalTokenData>(entry.Value);
            }
        }

        public virtual async Task<string> SetAsync(ExternalTokenData value, TimeSpan validFor)
        {
            var key = Guid.NewGuid().ToString("N");

            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = new IdentityCache
                {
                    Key = key,
                    CacheType = CacheType.ExternalTokenData,
                    ValidUntil = DateTime.UtcNow.Add(validFor),
                    Value = JsonConvert.SerializeObject(value),
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }

            return key;
        }

        public virtual async Task UpdateAsync(string key, ExternalTokenData value, TimeSpan validFor)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = await cacheSet
                                .Where(p => p.CacheType == CacheType.ExternalTokenData && p.Key == key && !p.Disabled)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

                if (entry == null)
                {
                    _logKeyNotFound(_logger, key, null);
                    throw new CacheEntryNotFoundException();
                }

                entry.ValidUntil = DateTime.UtcNow.Add(validFor);
                entry.Value = JsonConvert.SerializeObject(value);

                await _context.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
            }
        }
    }
}
