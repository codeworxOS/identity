using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    internal class ClientCredentialsParameters : IClientCredentialsParameters
    {
        public ClientCredentialsParameters(IClientRegistration client, string[] scopes,  ClaimsIdentity user)
        {
            Client = client;
            Scopes = scopes.ToImmutableList();
            User = user;
        }

        public IClientRegistration Client { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public ClaimsIdentity User { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}