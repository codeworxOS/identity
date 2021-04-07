using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;

namespace Codeworx.Identity.OAuth.Token
{
    internal class ClientCredentialsParameters : IClientCredentialsParameters
    {
        public ClientCredentialsParameters(string clientId, string clientSecret, string nonce, string[] scopes, string state, TimeSpan tokenExpiration, ClaimsIdentity user)
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Nonce = nonce;
            Scopes = scopes.ToImmutableList();
            State = state;
            TokenExpiration = tokenExpiration;
            User = user;
        }

        public string ClientSecret { get; }

        public TimeSpan TokenExpiration { get; }

        public string ClientId { get; }

        public string Nonce { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public string State { get; }

        public ClaimsIdentity User { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}