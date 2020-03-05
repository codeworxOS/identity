namespace Codeworx.Identity.OAuth.Token
{
    public class UnsupportedGrantTypeResult : ITokenResult
    {
        public UnsupportedGrantTypeResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.UnsupportedGrantType, string.Empty, string.Empty);
        }

        public TokenErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
