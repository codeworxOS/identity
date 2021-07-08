using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;

namespace Codeworx.Identity.OAuth.Token
{
    internal class RefreshTokenParameters : IIdentityDataParameters
    {
        public RefreshTokenParameters(string clientId, string[] scopes, ClaimsIdentity user)
        {
            ClientId = clientId;
            Scopes = scopes.ToImmutableList();
            User = user;
        }

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