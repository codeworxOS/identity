using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    // EventIds 140xx
    public class AuthorizationCodeCache<TContext> : IAuthorizationCodeCache
        where TContext : DbContext
    {
        private static readonly Action<ILogger, string, Exception> _logKeyExists;
        private static readonly Action<ILogger, string, Exception> _logKeyExpired;
        private static readonly Action<ILogger, string, Exception> _logKeyAlreadyUsed;
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;
        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;
        private readonly TContext _context;
        private readonly ILogger<AuthorizationCodeCache<TContext>> _logger;
        private readonly IAuthorizationCodeGenerator _codeGenerator;
        private readonly ISymmetricDataEncryption _dataEncryption;

        static AuthorizationCodeCache()
        {
            _logKeyExists = LoggerMessage.Define<string>(LogLevel.Error, new EventId(14001), "The Authentication Code {Key}, already exists.");
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14002), "The Authentication Code {Key} is expired");
            _logKeyAlreadyUsed = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14003), "The Authentication Code {Key} has already been used");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14004), "The Authentication Code {Key} was not found!");

            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(14005), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(14006), "The format of cache key {Key} is invalid!");
        }

        public AuthorizationCodeCache(
            TContext context,
            ILogger<AuthorizationCodeCache<TContext>> logger,
            IAuthorizationCodeGenerator codeGenerator,
            ISymmetricDataEncryption dataEncryption)
        {
            _context = context;
            _logger = logger;
            _codeGenerator = codeGenerator;
            _dataEncryption = dataEncryption;
        }

        public virtual async Task<IdentityData> GetAsync(string authorizationCode)
        {
            string cacheKey, encryptedKey;
            GetKeys(authorizationCode, out cacheKey, out encryptedKey);

            var cacheSet = _context.Set<IdentityCache>();

            var entry = await cacheSet
                            .Where(p => p.CacheType == CacheType.AuthorizationCode && p.Key == cacheKey)
                            .FirstOrDefaultAsync()
                            .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, cacheKey, null);
            }
            else if (entry.Disabled)
            {
                _logKeyAlreadyUsed(_logger, cacheKey, null);
                entry = null;
            }
            else if (entry.ValidUntil < DateTime.UtcNow)
            {
                _logKeyExpired(_logger, cacheKey, null);
                entry = null;
            }

            if (entry == null)
            {
                throw new CacheEntryNotFoundException();
            }

            var data = await _dataEncryption.DecryptAsync(entry.Value, encryptedKey);

            return JsonConvert.DeserializeObject<IdentityData>(data);
        }

        public virtual async Task<string> SetAsync(IdentityData payload, TimeSpan validFor)
        {
            var cacheKey = await _codeGenerator.GenerateCode();

            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                if (await cacheSet.AnyAsync(p => p.Key == cacheKey).ConfigureAwait(false))
                {
                    var exception = new CacheKeyAlreadyExistsException();
                    _logKeyExists(_logger, cacheKey, exception);
                    throw exception;
                }

                var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(payload));

                var entry = new IdentityCache
                {
                    Key = cacheKey,
                    CacheType = CacheType.AuthorizationCode,
                    ValidUntil = DateTime.UtcNow.Add(validFor),
                    Value = encrypted.Data,
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();

                return $"{cacheKey}.{encrypted.Key}";
            }
        }

        private void GetKeys(string key, out string cacheKey, out string encryptionKey)
        {
            var splitKey = key.Split('.');
            if (splitKey.Length != 2)
            {
                var exception = new InvalidCacheKeyFormatException();
                _logInvalidKeyFormat(_logger, exception);
                _logInvalidKeyFormatTrace(_logger, key, exception);

                throw exception;
            }

            cacheKey = splitKey[0];
            encryptionKey = splitKey[1];
        }
    }
}
