using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    internal class RefreshTokenParameters : IRefreshTokenParameters
    {
        public RefreshTokenParameters(IClientRegistration client, string clientSecret, string refreshToken, string[] scopes, ClaimsIdentity user, IRefreshTokenCacheItem cacheItem)
        {
            Client = client;
            Scopes = scopes.ToImmutableList();
            User = user;
            ClientSecret = clientSecret;
            RefreshToken = refreshToken;
            CacheItem = cacheItem;
        }

        public IClientRegistration Client { get; }

        public string Nonce { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public string State { get; }

        public ClaimsIdentity User { get; }

        public string RefreshToken { get; }

        public string ClientSecret { get; }

        public IRefreshTokenCacheItem CacheItem { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}