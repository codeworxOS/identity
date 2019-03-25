namespace Codeworx.Identity.OAuth.Token
{
    public class InvalidGrantResult : ITokenResult
    {
        public InvalidGrantResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidGrant, string.Empty, string.Empty);
        }

        public TokenErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
