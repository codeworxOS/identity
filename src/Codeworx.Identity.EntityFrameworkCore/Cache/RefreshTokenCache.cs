using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    // EventIds 150xx
    public class RefreshTokenCache<TContext> : IRefreshTokenCache
        where TContext : DbContext
    {
        private static readonly Action<ILogger, string, Exception> _logKeyExpired;
        private static readonly Action<ILogger, string, Exception> _logKeyNotFound;
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;
        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;
        private readonly TContext _context;
        private readonly ISymmetricDataEncryption _dataEncryption;
        private readonly ILogger<RefreshTokenCache<TContext>> _logger;

        static RefreshTokenCache()
        {
            _logKeyExpired = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(15001), "The Authentication Code {Key} is expired");
            _logKeyNotFound = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(15002), "The Authentication Code {Key} was not found!");

            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(15003), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(15004), "The format of cache key {Key} is invalid!");
        }

        public RefreshTokenCache(
            TContext context,
            ISymmetricDataEncryption dataEncryption,
            ILogger<RefreshTokenCache<TContext>> logger)
        {
            _context = context;
            _dataEncryption = dataEncryption;
            _logger = logger;
        }

        public Task ExtendLifetimeAsync(string key, TimeSpan extendBy, CancellationToken token = default)
        {
            throw new NotImplementedException();
        }

        public async Task<IRefreshTokenCacheItem> GetAsync(string key, CancellationToken token = default)
        {
            string cacheKey, encryptionKey;
            GetKeys(key, out cacheKey, out encryptionKey);

            var cacheSet = _context.Set<UserRefreshToken>();

            var entry = await cacheSet
                            .Where(p => p.Token == cacheKey)
                            .FirstOrDefaultAsync(token)
                            .ConfigureAwait(false);

            if (entry == null)
            {
                _logKeyNotFound(_logger, cacheKey, null);
            }
            else if (entry.IsDisabled || entry.ValidUntil < DateTime.UtcNow)
            {
                _logKeyExpired(_logger, cacheKey, null);
                entry = null;
            }

            if (entry == null)
            {
                throw new CacheEntryNotFoundException();
            }

            var decrypted = await _dataEncryption.DecryptAsync(entry.IdentityData, encryptionKey).ConfigureAwait(false);

            var result = JsonConvert.DeserializeObject<IdentityData>(decrypted);

            return new RefreshTokenCacheItem(result, entry.ValidUntil);
        }

        public async Task<string> SetAsync(IdentityData data, TimeSpan validFor, CancellationToken token = default)
        {
            using (var transaction = await _context.Database.BeginTransactionAsync(token).ConfigureAwait(false))
            {
                var cacheKey = Guid.NewGuid().ToString("N");
                var userId = Guid.Parse(data.Identifier);
                var clientId = Guid.Parse(data.ClientId);

                var set = _context.Set<UserRefreshToken>();

                var encrypted = await _dataEncryption.EncryptAsync(JsonConvert.SerializeObject(data)).ConfigureAwait(false);

                var refreshToken = new UserRefreshToken
                {
                    Token = cacheKey,
                    UserId = userId,
                    ClientId = clientId,
                    ValidUntil = DateTime.UtcNow.Add(validFor),
                    IdentityData = encrypted.Data,
                };

                set.Add(refreshToken);

                await _context.SaveChangesAsync(token).ConfigureAwait(false);
                await transaction.CommitAsync(token).ConfigureAwait(false);

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

        private class RefreshTokenCacheItem : IRefreshTokenCacheItem
        {
            public RefreshTokenCacheItem(IdentityData identityData, DateTime validUntil)
            {
                IdentityData = identityData;
                ValidUntil = validUntil;
            }

            public IdentityData IdentityData { get; }

            public DateTime ValidUntil { get; }
        }
    }
}
