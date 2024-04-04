using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    internal class TokenExchangeParameters : ITokenExchangeParameters
    {
        private readonly DateTimeOffset _validFrom;

        public TokenExchangeParameters(
            IClientRegistration client,
            string[] scopes,
            ClaimsIdentity user,
            IUser identityUser,
            string audience,
            string subjectToken,
            string subjectTokenType,
            string actorToken,
            string actorTokenType,
            string[] requestedTokenTypes)
        {
            Client = client;
            Scopes = scopes?.ToImmutableList() ?? ImmutableList<string>.Empty;
            User = user;
            IdentityUser = identityUser;
            Audience = audience;
            SubjectToken = subjectToken;
            SubjectTokenType = subjectTokenType;
            ActorToken = actorToken;
            ActorTokenType = actorTokenType;
            RequestedTokenTypes = requestedTokenTypes?.ToImmutableList() ?? ImmutableList<string>.Empty;
            _validFrom = DateTimeOffset.UtcNow;
        }

        public string ActorToken { get; }

        public string ActorTokenType { get; }

        public string Audience { get; }

        public IClientRegistration Client { get; }

        public string ClientSecret { get; }

        public MfaFlowMode MfaFlowMode => MfaFlowMode.Enabled;

        public IReadOnlyCollection<string> RequestedTokenTypes { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public string SubjectToken { get; }

        public string SubjectTokenType { get; }

        public DateTimeOffset TokenValidUntil => _validFrom.Add(Client.TokenExpiration);

        public ClaimsIdentity User { get; }

        public IUser IdentityUser { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}