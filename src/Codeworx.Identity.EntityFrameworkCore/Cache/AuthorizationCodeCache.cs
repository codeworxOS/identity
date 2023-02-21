using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.OAuth;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.EntityFrameworkCore.Cache
{
    public class AuthorizationCodeCache<TContext> : EncryptedCache<TContext, IdentityData>, IAuthorizationCodeCache
        where TContext : DbContext
    {
        private readonly IAuthorizationCodeGenerator _codeGenerator;

        public AuthorizationCodeCache(
            TContext context,
            ILogger<AuthorizationCodeCache<TContext>> logger,
            IAuthorizationCodeGenerator codeGenerator,
            ISymmetricDataEncryption dataEncryption)
            : base(context, logger, dataEncryption)
        {
            _codeGenerator = codeGenerator;
        }

        public virtual async Task<IdentityData> GetAsync(string authorizationCode, CancellationToken token = default)
        {
            var entry = await GetEntryAsync(CacheType.AuthorizationCode, authorizationCode, true, token);
            return entry.Data;
        }

        public virtual async Task<string> SetAsync(IdentityData payload, TimeSpan validFor, CancellationToken token = default)
        {
            var cacheKey = await _codeGenerator.GenerateCode();
            var validUntil = DateTimeOffset.UtcNow.Add(validFor);
            return await AddEntryAsync(CacheType.AuthorizationCode, cacheKey, payload, validUntil, token);
        }

        protected override Guid? GetUserId(IdentityData data)
        {
            return Guid.Parse(data.Identifier);
        }
    }
}
