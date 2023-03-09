using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Invitation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    // EventIds 142xx
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

        public virtual async Task<InvitationItem> RedeemAsync(string code)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<UserInvitation>();

                var entry = await GetEntryAsync(code, cacheSet);
                entry.IsDisabled = true;

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();

                return new InvitationItem
                {
                    UserId = entry.UserId.ToString("N"),
                    RedirectUri = entry.RedirectUri,
                    Action = entry.Action,
                };
            }
        }

        public virtual async Task AddAsync(string code, InvitationItem factory, TimeSpan validity)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<UserInvitation>();

                if (await cacheSet.AnyAsync(p => p.InvitationCode == code).ConfigureAwait(false))
                {
                    var exception = new CacheKeyAlreadyExistsException();
                    _logKeyExists(_logger, code, exception);
                    throw exception;
                }

                var entry = new UserInvitation
                {
                    InvitationCode = code,
                    Action = factory.Action,
                    IsDisabled = false,
                    ValidUntil = DateTime.UtcNow.Add(validity),
                    RedirectUri = factory.RedirectUri,
                    UserId = Guid.Parse(factory.UserId),
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }
        }

        public virtual async Task<InvitationItem> GetAsync(string code)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<UserInvitation>();
                UserInvitation entry = await GetEntryAsync(code, cacheSet).ConfigureAwait(false);

                transaction.Commit();

                return new InvitationItem
                {
                    UserId = entry.UserId.ToString("N"),
                    RedirectUri = entry.RedirectUri,
                    Action = entry.Action,
                };
            }
        }

        private async Task<UserInvitation> GetEntryAsync(string code, DbSet<UserInvitation> cacheSet)
        {
            var entry = await cacheSet
                                            .Where(p => p.InvitationCode == code)
                                            .FirstOrDefaultAsync()
                                            .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, code, null);
                throw new InvitationNotFoundException();
            }
            else if (entry.IsDisabled)
            {
                _logKeyAlreadyUsed(_logger, code, null);
                throw new InvitationAlreadyRedeemedException();
            }
            else if (entry.ValidUntil < DateTime.UtcNow)
            {
                _logKeyExpired(_logger, code, null);
                throw new InvitationExpiredException();
            }

            return entry;
        }
    }
}
