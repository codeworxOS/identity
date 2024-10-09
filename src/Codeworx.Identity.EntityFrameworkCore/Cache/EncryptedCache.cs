using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    // EventIds 140xx
    public abstract partial class EncryptedCache<TContext, TData>
        where TContext : Microsoft.EntityFrameworkCore.DbContext
        where TData : class
    {
        private readonly TContext _context;

        private readonly ISymmetricDataEncryption _dataEncryption;

        private readonly ILogger _logger;

        public EncryptedCache(
            TContext context,
            ILogger logger,
            ISymmetricDataEncryption dataEncryption)
        {
            _context = context;
            _logger = logger;
            _dataEncryption = dataEncryption;
        }

        [LoggerMessage(
                    Level = LogLevel.Error,
            EventId = 14005,
            Message = "The format of cache key is invalid!")]
        public static partial void LogInvalidKeyFormat(ILogger logger, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Trace,
            EventId = 14006,
            Message = "The format of cache key {key} is invalid!")]
        public static partial void LogInvalidKeyFormatTrace(ILogger logger, string key, Exception ex);

        [LoggerMessage(
            Level = LogLevel.Warning,
            EventId = 14003,
            Message = "The cache key {key} has already been used")]
        public static partial void LogKeyAlreadyUsed(ILogger logger, string key);

        [LoggerMessage(Level = LogLevel.Error,
                                                                            EventId = 14001,
            Message = "The cache key {key}, already exists.")]
        public static partial void LogKeyExists(ILogger logger, string key, Exception ex);

        [LoggerMessage(Level = LogLevel.Warning,
            EventId = 14002,
            Message = "The cache key {key} is expired")]
        public static partial void LogKeyExpired(ILogger logger, string key);

        [LoggerMessage(
            Level = LogLevel.Warning,
            EventId = 14004,
            Message = "The Authentication Code {Key} was not found!")]
        public static partial void LogKeyNotFound(ILogger logger, string key);

        protected virtual async Task<CacheEntry<TData>> GetEntryAsync(CacheType cacheType, string key, bool disableOnReceive, CancellationToken token = default)
        {
            string cacheKey, encryptedKey;
            GetKeys(key, out cacheKey, out encryptedKey);

            string data = null;
            DateTimeOffset validUntil;

            await using (var transaction = await _context.Database.EnsureTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                var entry = await cacheSet
                                .Where(p => p.CacheType == cacheType && p.Key == cacheKey)
                                .FirstOrDefaultAsync()
                                .ConfigureAwait(false);

                if (entry == null)
                {
                    LogKeyNotFound(_logger, cacheKey);
                }
                else if (entry.Disabled)
                {
                    LogKeyAlreadyUsed(_logger, cacheKey);
                    entry = null;
                }
                else if (entry.ValidUntil < DateTime.UtcNow)
                {
                    LogKeyExpired(_logger, cacheKey);
                    entry = null;
                }

                if (entry == null)
                {
                    throw new CacheEntryNotFoundException();
                }

                data = entry.Value;
                validUntil = new DateTimeOffset(entry.ValidUntil, TimeSpan.Zero);

                if (disableOnReceive)
                {
                    entry.Disabled = true;
                    await _context.SaveChangesAsync().ConfigureAwait(false);
                    await transaction.CommitAsync().ConfigureAwait(false);
                }
            }

            var decrypted = await _dataEncryption.DecryptAsync(data, encryptedKey);
            var result = JsonConvert.DeserializeObject<TData>(decrypted);

            return new CacheEntry<TData>(result, validUntil);
        }

        protected abstract Guid? GetUserId(TData data);

        protected virtual async Task<string> UpdateEntryAsync(CacheType cacheType, string key, TData payload, CancellationToken token = default)
        {
            string cacheKey, encryptedKey;
            GetKeys(key, out cacheKey, out encryptedKey);

            using (var transaction = await _context.Database.EnsureTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();
                var entry = await cacheSet.FirstOrDefaultAsync(p => p.CacheType == cacheType && p.Key == cacheKey).ConfigureAwait(false);

                if (entry == null)
                {
                    LogKeyNotFound(_logger, cacheKey);
                    throw new CacheEntryNotFoundException();
                }

                var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(payload), encryptedKey);

                entry.Value = encrypted.Data;

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();

                return $"{cacheKey}.{encrypted.Key}";
            }
        }

        protected virtual async Task<string> ExtendEntryAsync(CacheType cacheType, string key, DateTimeOffset validUntil, CancellationToken token = default)
        {
            string cacheKey, encryptedKey;
            GetKeys(key, out cacheKey, out encryptedKey);

            using (var transaction = await _context.Database.EnsureTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();
                var entry = await cacheSet.FirstOrDefaultAsync(p => p.CacheType == cacheType && p.Key == cacheKey).ConfigureAwait(false);

                if (entry == null)
                {
                    LogKeyNotFound(_logger, cacheKey);
                    throw new CacheEntryNotFoundException();
                }

                entry.ValidUntil = validUntil.UtcDateTime;

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();

                return $"{cacheKey}.{encryptedKey}";
            }
        }

        protected virtual async Task<string> AddEntryAsync(CacheType cacheType, string cacheKey, TData payload, DateTimeOffset validUntil, CancellationToken token = default)
        {
            using (var transaction = await _context.Database.EnsureTransactionAsync().ConfigureAwait(false))
            {
                var cacheSet = _context.Set<IdentityCache>();

                if (await cacheSet.AnyAsync(p => p.Key == cacheKey).ConfigureAwait(false))
                {
                    var exception = new CacheKeyAlreadyExistsException();
                    LogKeyExists(_logger, cacheKey, exception);
                    throw exception;
                }

                var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(payload));

                var entry = new IdentityCache
                {
                    Key = cacheKey,
                    CacheType = cacheType,
                    UserId = GetUserId(payload),
                    ValidUntil = validUntil.UtcDateTime,
                    Value = encrypted.Data,
                };

                await cacheSet.AddAsync(entry).ConfigureAwait(false);

                await _context.SaveChangesAsync().ConfigureAwait(false);

                transaction.Commit();

                return $"{cacheKey}.{encrypted.Key}";
            }
        }

        protected virtual void GetKeys(string key, out string cacheKey, out string encryptionKey)
        {
            var splitKey = key.Split('.');
            if (splitKey.Length != 2)
            {
                var exception = new InvalidCacheKeyFormatException();
                LogInvalidKeyFormat(_logger, exception);
                LogInvalidKeyFormatTrace(_logger, key, exception);

                throw exception;
            }

            cacheKey = splitKey[0];
            encryptionKey = splitKey[1];
        }
    }
}
