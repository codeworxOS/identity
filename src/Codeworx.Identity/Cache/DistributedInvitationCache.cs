using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.Invitation;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace Codeworx.Identity.Cache
{
    // EventIds 157xx
    public class DistributedInvitationCache : IInvitationCache
    {
        private static readonly Action<ILogger, Exception> _logInvalidKeyFormat;

        private static readonly Action<ILogger, string, Exception> _logInvalidKeyFormatTrace;

        private readonly IDistributedCache _cache;

        private readonly ISymmetricDataEncryption _dataEncryption;

        private readonly ILogger<DistributedInvitationCache> _logger;

        static DistributedInvitationCache()
        {
            _logInvalidKeyFormat = LoggerMessage.Define(LogLevel.Error, new EventId(15703), "The format of cache key is invalid!");
            _logInvalidKeyFormatTrace = LoggerMessage.Define<string>(
                LogLevel.Trace,
                new EventId(15704),
                "The format of cache key {Key} is invalid!");
        }

        public DistributedInvitationCache(
            IDistributedCache cache,
            ISymmetricDataEncryption dataEncryption,
            ILogger<DistributedInvitationCache> logger)
        {
            _cache = cache;
            _dataEncryption = dataEncryption;
            _logger = logger;
        }

        public async Task AddAsync(string code, InvitationItem factory, TimeSpan validity)
        {
            var cacheKey = $"identity_invitation_{code}";

            var entry = new TokenCacheEntry { Data = factory, ValidUntil = DateTimeOffset.UtcNow.Add(validity) };

            await _cache.SetStringAsync(
                cacheKey,
                JsonConvert.SerializeObject(entry),
                new DistributedCacheEntryOptions() { AbsoluteExpiration = entry.ValidUntil });
        }

        public async Task<InvitationItem> GetAsync(string code)
        {
            var cacheKey = $"identity_invitation_{code}";
            var cachedInvitation = await _cache.GetStringAsync(cacheKey).ConfigureAwait(false);
            if (cachedInvitation == null)
            {
                throw new InvitationNotFoundException();
            }

            var entry = JsonConvert.DeserializeObject<TokenCacheEntry>(cachedInvitation);

            if (entry.ValidUntil < DateTimeOffset.UtcNow)
            {
                throw new InvitationExpiredException(entry.Data);
            }

            if (entry.Redeemed)
            {
                throw new InvitationAlreadyRedeemedException();
            }

            return entry.Data;
        }

        public async Task<InvitationItem> RedeemAsync(string code)
        {
            var cacheKey = $"identity_invitation_{code}";

            var cachedInvitation = await _cache.GetStringAsync(cacheKey).ConfigureAwait(false);
            if (cachedInvitation == null)
            {
                throw new InvitationNotFoundException();
            }

            var entry = JsonConvert.DeserializeObject<TokenCacheEntry>(cachedInvitation);

            if (entry.ValidUntil < DateTimeOffset.UtcNow)
            {
                throw new InvitationExpiredException(entry.Data);
            }

            if (entry.Redeemed)
            {
                throw new InvitationAlreadyRedeemedException();
            }

            entry.Redeemed = true;

            await _cache.SetStringAsync(
               cacheKey,
               JsonConvert.SerializeObject(entry),
               new DistributedCacheEntryOptions() { AbsoluteExpiration = entry.ValidUntil });

            return entry.Data;
        }

        private class TokenCacheEntry
        {
            public TokenCacheEntry()
            {
            }

            public bool Redeemed { get; set; }

            public InvitationItem Data { get; set; }

            public DateTimeOffset ValidUntil { get; set; }
        }
    }
}