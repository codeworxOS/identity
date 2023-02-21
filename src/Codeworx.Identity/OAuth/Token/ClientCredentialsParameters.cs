using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    internal class ClientCredentialsParameters : IClientCredentialsParameters
    {
        private readonly DateTimeOffset _validFrom;

        public ClientCredentialsParameters(IClientRegistration client, string[] scopes, ClaimsIdentity user)
        {
            Client = client;
            Scopes = scopes.ToImmutableList();
            User = user;

            _validFrom = DateTimeOffset.UtcNow;
        }

        public IClientRegistration Client { get; }

        public MfaFlowMode MfaFlowModel => MfaFlowMode.Disabled;

        public IReadOnlyCollection<string> Scopes { get; }

        public DateTimeOffset TokenValidUntil => _validFrom.Add(Client.TokenExpiration);

        public ClaimsIdentity User { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}