using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    internal class ClientCredentialsParameters : IClientCredentialsParameters
    {
        public ClientCredentialsParameters(IClientRegistration client, string clientSecret, string nonce, string[] scopes, string state, ClaimsIdentity user)
        {
            Client = client;
            ClientSecret = clientSecret;
            Nonce = nonce;
            Scopes = scopes.ToImmutableList();
            State = state;
            User = user;
        }

        public string ClientSecret { get; }

        public TimeSpan TokenExpiration { get; }

        public IClientRegistration Client { get; }

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