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

        public ClientCredentialsParameters(IClientRegistration client, string[] scopes, ClaimsIdentity user, IUser identityUser)
        {
            Client = client;
            Scopes = scopes.ToImmutableList();
            User = user;
            IdentityUser = identityUser;

            _validFrom = DateTimeOffset.UtcNow;
            TokenValidUntil = _validFrom.Add(Client.TokenExpiration);
        }

        public ClientCredentialsParameters(IClientRegistration client, string[] scopes, ClaimsIdentity user, IUser identityUser, DateTimeOffset validUntil)
            : this(client, scopes, user, identityUser)
        {
            TokenValidUntil = validUntil;
        }

        public IClientRegistration Client { get; }

        public MfaFlowMode MfaFlowModel => MfaFlowMode.Disabled;

        public IReadOnlyCollection<string> Scopes { get; }

        public DateTimeOffset TokenValidUntil { get; }

        public ClaimsIdentity User { get; }

        public IUser IdentityUser { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}