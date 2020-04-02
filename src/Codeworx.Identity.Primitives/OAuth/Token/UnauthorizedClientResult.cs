namespace Codeworx.Identity.OAuth.Token
{
    public class UnauthorizedClientResult : ITokenResult
    {
        public UnauthorizedClientResult()
        {
            this.Error = new ErrorResponse(Constants.Error.UnauthorizedClient, string.Empty, string.Empty);
        }

        public ErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
