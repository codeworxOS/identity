using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    public class StateLookupCache<TContext> : IStateLookupCache
        where TContext : DbContext
    {
        private static readonly Action<ILogger, string, Exception> _logKeyExists;
        private static readonly Action<ILogger, string, Exception> _logKeyExpired;
        private static readonly Action<ILogger, string, Exception> _logKeyAlreadyUsed;
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;

        private readonly TContext _context;
        private readonly ILogger<AuthorizationCodeCache<TContext>> _logger;

        static StateLookupCache()
        {
            _logKeyExists = LoggerMessage.Define<string>(LogLevel.Error, new EventId(14101), "The State {Key}, already exists.");
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14102), "The State {Key} is expired");
            _logKeyAlreadyUsed = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14103), "The State {Key} has already been used");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14104), "The State {Key} not found!");
        }

        public StateLookupCache(TContext context, ILogger<AuthorizationCodeCache<TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<string> GetAsync(string state)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = await cacheSet
                                .Where(p => p.CacheType == CacheType.Lookup && p.Key == state)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

                if (entry == null)
                {
                    _logKeyNotFound(_logger, state, null);
                }
                else if (entry.Disabled)
                {
                    _logKeyAlreadyUsed(_logger, state, null);
                    entry = null;
                }
                else if (entry.ValidUntil < DateTime.UtcNow)
                {
                    _logKeyExpired(_logger, state, null);
                    entry = null;
                }

                if (entry == null)
                {
                    throw new CacheEntryNotFoundException();
                }

                entry.Disabled = true;
                await _context.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();

                return entry.Value;
            }
        }

        public async Task SetAsync(string state, string value)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                if (await cacheSet.AnyAsync(p => p.Key == state).ConfigureAwait(false))
                {
                    var exception = new CacheKeyAlreadyExistsException();
                    _logKeyExists(_logger, state, exception);
                    throw exception;
                }

                var entry = new IdentityCache
                {
                    Key = state,
                    CacheType = CacheType.Lookup,
                    ValidUntil = DateTime.UtcNow.Add(TimeSpan.FromMinutes(5)),
                    Value = value,
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }
        }
    }
}
