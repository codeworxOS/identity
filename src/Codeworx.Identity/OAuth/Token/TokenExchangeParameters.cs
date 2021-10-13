using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    internal class TokenExchangeParameters : ITokenExchangeParameters
    {
        public TokenExchangeParameters(
            IClientRegistration client,
            string[] scopes,
            ClaimsIdentity user,
            string audience,
            string subjectToken,
            string subjectTokenType,
            string actorToken,
            string actorTokenType)
        {
            Client = client;
            Scopes = scopes.ToImmutableList();
            User = user;
            Audience = audience;
            SubjectToken = subjectToken;
            SubjectTokenType = subjectTokenType;
            ActorToken = actorToken;
            ActorTokenType = actorTokenType;
        }

        public IClientRegistration Client { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public ClaimsIdentity User { get; }

        public string ClientSecret { get; }

        public string SubjectToken { get; }

        public string SubjectTokenType { get; }

        public string ActorToken { get; }

        public string ActorTokenType { get; }

        public string Audience { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}