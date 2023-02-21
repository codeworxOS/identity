using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.OAuth;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.Cache
{
    public class DistributedAuthorizationCodeCache : DistributedEncryptedCache<IdentityData>, IAuthorizationCodeCache
    {
        private readonly IAuthorizationCodeGenerator _codeGenerator;

        public DistributedAuthorizationCodeCache(
            IDistributedCache cache,
            ILogger<DistributedAuthorizationCodeCache> logger,
            IAuthorizationCodeGenerator codeGenerator,
            ISymmetricDataEncryption dataEncryption)
            : base(cache, logger, dataEncryption)
        {
            _codeGenerator = codeGenerator;
        }

        protected override string CacheKeyPrefix => "Identity_AuthorizationCode";

        public async Task<IdentityData> GetAsync(string authorizationCode, CancellationToken token = default)
        {
            return await GetEntryAsync(authorizationCode, token);
        }

        public async Task<string> SetAsync(IdentityData payload, TimeSpan validFor, CancellationToken token = default)
        {
            var cacheKey = await _codeGenerator.GenerateCode().ConfigureAwait(false);

            var validUntil = DateTimeOffset.UtcNow.Add(validFor);

            return await AddEntryAsync(cacheKey, payload, validUntil, token);
        }
    }
}