using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    internal class RefreshTokenParameters : IRefreshTokenParameters
    {
        private readonly DateTimeOffset _validFrom;

        public RefreshTokenParameters(IClientRegistration client, string refreshToken, string[] scopes, ClaimsIdentity user, IUser identityUser, IToken parsedRefreshToken)
        {
            Client = client;
            Scopes = scopes.ToImmutableList();
            User = user;
            IdentityUser = identityUser;
            RefreshToken = refreshToken;
            ParsedRefreshToken = parsedRefreshToken;
            _validFrom = DateTimeOffset.UtcNow;
        }

        public IClientRegistration Client { get; }

        public MfaFlowMode MfaFlowModel => MfaFlowMode.Enabled;

        public IToken ParsedRefreshToken { get; }

        public string RefreshToken { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public DateTimeOffset TokenValidUntil => _validFrom.Add(Client.TokenExpiration);

        public ClaimsIdentity User { get; }

        public IUser IdentityUser { get; }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}