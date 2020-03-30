using System;

namespace Codeworx.Identity.OAuth.Token
{
    public class SuccessfulTokenResult : ITokenResult
    {
        public SuccessfulTokenResult(string accessToken, string idToken, TimeSpan expiresIn, string scope)
        {
            this.Response = new TokenResponse(accessToken, idToken, Constants.TokenType.Bearer, (int)expiresIn.TotalSeconds, scope);
        }

        public TokenErrorResponse Error => null;

        public TokenResponse Response { get; }
    }
}
