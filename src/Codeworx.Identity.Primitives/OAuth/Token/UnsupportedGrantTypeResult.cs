namespace Codeworx.Identity.OAuth.Token
{
    public class UnsupportedGrantTypeResult : ITokenResult
    {
        public UnsupportedGrantTypeResult()
        {
            this.Error = new ErrorResponse(Constants.Error.UnsupportedGrantType, string.Empty, string.Empty);
        }

        public ErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
