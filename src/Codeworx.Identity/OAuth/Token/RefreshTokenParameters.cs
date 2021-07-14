using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Cache;

namespace Codeworx.Identity.OAuth.Token
{
    internal class RefreshTokenParameters : IRefreshTokenParameters
    {
        public RefreshTokenParameters(string clientId, string clientSecret, string refreshToken, string[] scopes, ClaimsIdentity user, IRefreshTokenCacheItem cacheItem, TimeSpan tokenExpiration)
        {
            ClientId = clientId;
            Scopes = scopes.ToImmutableList();
            User = user;
            ClientSecret = clientSecret;
            RefreshToken = refreshToken;
            CacheItem = cacheItem;
            TokenExpiration = tokenExpiration;
        }

        public string ClientId { get; }

        public string Nonce { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public string State { get; }

        public ClaimsIdentity User { get; }

        public string RefreshToken { get; }

        public string ClientSecret { get; }

        public IRefreshTokenCacheItem CacheItem { get; }

        public TimeSpan TokenExpiration { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}