using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    // EventIds 151xx
    public class MailMfaCodeCache<TContext> : IMailMfaCodeCache
        where TContext : DbContext
    {
        private static readonly Action<ILogger, string, Exception> _logKeyAlreadyUsed;
        private static readonly Action<ILogger, string, Exception> _logKeyExists;
        private static readonly Action<ILogger, string, Exception> _logKeyExpired;
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;
        private static readonly Random _random;
        private readonly TContext _context;
        private readonly ILogger<MailMfaCodeCache<TContext>> _logger;

        static MailMfaCodeCache()
        {
            _random = new Random();
            _logKeyExists = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(15104), "The mail mfa session {Key} already exists");
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(15101), "The mail mfa session {Key} is expired");
            _logKeyAlreadyUsed = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(15102), "The mail mfa session {Key} has already been used");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(15103), "The mail mfa session {Key} was not found!");
        }

        public MailMfaCodeCache(TContext context, ILogger<MailMfaCodeCache<TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> CreateAsync(string key, TimeSpan validFor, CancellationToken token = default)
        {
            var result = _random.Next(1000, 1000000).ToString().PadLeft(6, '0');
            var cacheKey = $"{Constants.Cache.MailMfaPrefix}_{key}";

            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var item = await cacheSet.FirstOrDefaultAsync(p => p.CacheType == CacheType.Lookup && p.Key == cacheKey).ConfigureAwait(false);

                if (item == null)
                {
                    item = new IdentityCache
                    {
                        Key = cacheKey,
                        CacheType = CacheType.Lookup,
                        Disabled = false,
                        ValidUntil = DateTime.UtcNow.Add(validFor),
                        Value = result,
                    };
                    cacheSet.Add(item);
                }
                else
                {
                    item.Value = result;
                    item.ValidUntil = DateTime.UtcNow.Add(validFor);
                    _logKeyExists(_logger, key, null);
                }

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }

            return result;
        }

        public async Task<string> GetAsync(string key, CancellationToken token = default)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheKey = $"{Constants.Cache.MailMfaPrefix}_{key}";

                var cacheSet = _context.Set<IdentityCache>();

                var entry = await cacheSet
                                .Where(p => p.CacheType == CacheType.Lookup && p.Key == cacheKey)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

                if (entry == null)
                {
                    _logKeyNotFound(_logger, key, null);
                }
                else if (entry.Disabled)
                {
                    _logKeyAlreadyUsed(_logger, key, null);
                    entry = null;
                }
                else if (entry.ValidUntil < DateTime.UtcNow)
                {
                    _logKeyExpired(_logger, key, null);
                    entry = null;
                }

                transaction.Commit();

                return entry?.Value;
            }
        }
    }
}
