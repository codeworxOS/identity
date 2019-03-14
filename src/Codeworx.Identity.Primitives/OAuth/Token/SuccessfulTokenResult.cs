namespace Codeworx.Identity.OAuth.Token
{
    public class SuccessfulTokenResult : ITokenResult
    {
        public SuccessfulTokenResult(string accessToken, string tokenType, int expiresIn = 0, string refreshToken = null, string scope = null)
        {
            this.Response = new TokenResponse(accessToken, tokenType, expiresIn, refreshToken, scope);
        }

        public TokenErrorResponse Error => null;

        public TokenResponse Response { get; }
    }
}
