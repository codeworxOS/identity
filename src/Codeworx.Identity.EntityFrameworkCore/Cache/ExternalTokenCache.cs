using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Cryptography;
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
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;
        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;
        private readonly TContext _context;
        private readonly ISymmetricDataEncryption _dataEncryption;
        private readonly ILogger<ExternalTokenCache<TContext>> _logger;

        static ExternalTokenCache()
        {
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14701), "The external token data {Key} is expired");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(14703), "The external token data {Key} was not found!");
            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(14704), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(14704), "The format of cache key {Key} is invalid!");
        }

        public ExternalTokenCache(TContext context, ISymmetricDataEncryption dataEncryption, ILogger<ExternalTokenCache<TContext>> logger)
        {
            _context = context;
            _dataEncryption = dataEncryption;
            _logger = logger;
        }

        public virtual async Task<ExternalTokenData> GetAsync(string key, TimeSpan extend)
        {
            string cacheKey, encryptionKey;
            GetKeys(key, out cacheKey, out encryptionKey);

            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = await cacheSet
                                .Where(p => p.CacheType == CacheType.ExternalTokenData && p.Key == cacheKey && !p.Disabled)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

                if (entry == null)
                {
                    _logKeyNotFound(_logger, cacheKey, null);
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

                entry.ValidUntil = DateTime.UtcNow.Add(extend);

                await _context.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
                var data = await _dataEncryption.DecryptAsync(entry.Value, encryptionKey);

                return JsonConvert.DeserializeObject<ExternalTokenData>(data);
            }
        }

        public virtual async Task<string> SetAsync(ExternalTokenData value, TimeSpan validFor)
        {
            var cacheKey = Guid.NewGuid().ToString("N");
            var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(value));

            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = new IdentityCache
                {
                    Key = cacheKey,
                    CacheType = CacheType.ExternalTokenData,
                    ValidUntil = DateTime.UtcNow.Add(validFor),
                    Value = encrypted.Data,
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();
            }

            return $"{cacheKey}.{encrypted.Key}";
        }

        public virtual async Task UpdateAsync(string key, ExternalTokenData value, TimeSpan validFor)
        {
            string cacheKey, encryptionKey;
            GetKeys(key, out cacheKey, out encryptionKey);

            using (var transaction = await _context.Database.BeginTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = await cacheSet
                                .Where(p => p.CacheType == CacheType.ExternalTokenData && p.Key == cacheKey && !p.Disabled)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

                if (entry == null)
                {
                    _logKeyNotFound(_logger, cacheKey, null);
                    throw new CacheEntryNotFoundException();
                }

                var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(value), encryptionKey);

                entry.ValidUntil = DateTime.UtcNow.Add(validFor);
                entry.Value = encrypted.Data;

                await _context.SaveChangesAsync().ConfigureAwait(false);
                transaction.Commit();
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
