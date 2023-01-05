using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    internal class RefreshTokenParameters : IRefreshTokenParameters
    {
        public RefreshTokenParameters(IClientRegistration client, string refreshToken, string[] scopes, ClaimsIdentity user, IToken parsedRefreshToken)
        {
            Client = client;
            Scopes = scopes.ToImmutableList();
            User = user;
            RefreshToken = refreshToken;
            ParsedRefreshToken = parsedRefreshToken;
        }

        public IClientRegistration Client { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public ClaimsIdentity User { get; }

        public string RefreshToken { get; }

        public IToken ParsedRefreshToken { get; }

        public MfaFlowMode MfaFlowModel => MfaFlowMode.Enabled;

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}