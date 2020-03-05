namespace Codeworx.Identity.OAuth.Token
{
    public class InvalidClientResult : ITokenResult
    {
        public InvalidClientResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidClient, string.Empty, string.Empty);
        }

        public TokenErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
