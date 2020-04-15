namespace Codeworx.Identity.OAuth.Token
{
    public class InvalidClientResult : ITokenResult
    {
        public InvalidClientResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidClient, string.Empty, string.Empty);
        }

        public ErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
