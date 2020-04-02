namespace Codeworx.Identity.OAuth.Token
{
    public class InvalidGrantResult : ITokenResult
    {
        public InvalidGrantResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidGrant, string.Empty, string.Empty);
        }

        public ErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
