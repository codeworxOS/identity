using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    public class AuthorizationCodeCache<TContext> : IAuthorizationCodeCache
        where TContext : DbContext
    {
        private static readonly Action<ILogger, string, Exception> _logKeyExists;
        private static readonly Action<ILogger, string, Exception> _logKeyExpired;
        private static readonly Action<ILogger, string, Exception> _logKeyAlreadyUsed;
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;

        private readonly TContext _context;
        private readonly ILogger<AuthorizationCodeCache<TContext>> _logger;

        static AuthorizationCodeCache()
        {
            _logKeyExists = LoggerMessage.Define<string>(LogLevel.Error, new EventId(14001), "The Authentication Code {Key}, already exists.");
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14002), "The Authentication Code {Key} is expired");
            _logKeyAlreadyUsed = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14002), "The Authentication Code {Key} has already been used");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14002), "The Authentication Code {Key} not found!");
        }

        public AuthorizationCodeCache(TContext context, ILogger<AuthorizationCodeCache<TContext>> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IdentityData> GetAsync(string authorizationCode)
        {
            var cacheSet = _context.Set<IdentityCache>();

            var entry = await cacheSet
                            .Where(p => p.CacheType == CacheType.AuthorizationCode && p.Key == authorizationCode)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, authorizationCode, null);
            }
            else if (entry.Disabled)
            {
                _logKeyAlreadyUsed(_logger, authorizationCode, null);
                entry = null;
            }
            else if (entry.ValidUntil < DateTime.UtcNow)
            {
                _logKeyExpired(_logger, authorizationCode, null);
                entry = null;
            }

            if (entry == null)
            {
                throw new CacheEntryNotFoundException();
            }

            var serializer = new JsonSerializer();
            using (var stringReader = new StringReader(entry.Value))
            using (var jsonReader = new JsonTextReader(stringReader))
            {
                return serializer.Deserialize<IdentityData>(jsonReader);
            }
        }

        public async Task SetAsync(string authorizationCode, IdentityData payload, TimeSpan timeout)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                if (await cacheSet.AnyAsync(p => p.Key == authorizationCode).ConfigureAwait(false))
                {
                    var exception = new CacheKeyAlreadyExistsException();
                    _logKeyExists(_logger, authorizationCode, exception);
                    throw exception;
                }

                JsonSerializer serializer = new JsonSerializer();
                var stringBuilder = new StringBuilder();
                using (var stringWriter = new StringWriter(stringBuilder))
                {
                    serializer.Serialize(stringWriter, payload);
                }

                var entry = new IdentityCache
                {
                    Key = authorizationCode,
                    CacheType = CacheType.AuthorizationCode,
                    ValidUntil = DateTime.UtcNow.Add(timeout),
                    Value = stringBuilder.ToString(),
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }
        }
    }
}
