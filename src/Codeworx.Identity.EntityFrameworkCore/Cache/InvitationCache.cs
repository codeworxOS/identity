using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Invitation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    public class InvitationCache<TContext> : IInvitationCache
        where TContext : DbContext
    {
        private static readonly Action<ILogger, string, Exception> _logKeyExists;
        private static readonly Action<ILogger, string, Exception> _logKeyExpired;
        private static readonly Action<ILogger, string, Exception> _logKeyAlreadyUsed;
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;

        private readonly TContext _context;
        private readonly ILogger<InvitationCache<TContext>> _logger;

        static InvitationCache()
        {
            _logKeyExists = LoggerMessage.Define<string>(LogLevel.Error, new EventId(14201), "The invitation {Key}, already exists.");
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14202), "The invitation {Key} is expired");
            _logKeyAlreadyUsed = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14203), "The invitation {Key} has already been used");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14204), "The invitation {Key} was not found!");
        }

        public InvitationCache(TContext context, ILogger<InvitationCache<TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task UpdateAsync(string code, Action<InvitationItem> update)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = await GetEntryAsync(code, cacheSet);
                var item = JsonConvert.DeserializeObject<InvitationItem>(entry.Value);
                update(item);
                entry.Value = JsonConvert.SerializeObject(item);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }
        }

        public async Task AddAsync(string code, InvitationItem factory, TimeSpan validity)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                if (await cacheSet.AnyAsync(p => p.Key == code).ConfigureAwait(false))
                {
                    var exception = new CacheKeyAlreadyExistsException();
                    _logKeyExists(_logger, code, exception);
                    throw exception;
                }

                var entry = new IdentityCache
                {
                    Key = code,
                    CacheType = CacheType.Invitation,
                    ValidUntil = DateTime.UtcNow.Add(validity),
                    Value = JsonConvert.SerializeObject(factory),
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }
        }

        public async Task<InvitationItem> GetAsync(string code)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();
                IdentityCache entry = await GetEntryAsync(code, cacheSet).ConfigureAwait(false);

                transaction.Commit();

                var result = JsonConvert.DeserializeObject<InvitationItem>(entry.Value);

                return result;
            }
        }

        private async Task<IdentityCache> GetEntryAsync(string code, DbSet<IdentityCache> cacheSet)
        {
            var entry = await cacheSet
                                            .Where(p => p.CacheType == CacheType.Invitation && p.Key == code)
                                            .FirstOrDefaultAsync()
                                            .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, code, null);
            }
            else if (entry.Disabled)
            {
                _logKeyAlreadyUsed(_logger, code, null);
                entry = null;
            }
            else if (entry.ValidUntil < DateTime.UtcNow)
            {
                _logKeyExpired(_logger, code, null);
                entry = null;
            }

            if (entry == null)
            {
                throw new CacheEntryNotFoundException();
            }

            return entry;
        }
    }
}
