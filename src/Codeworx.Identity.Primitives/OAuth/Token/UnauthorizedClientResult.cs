namespace Codeworx.Identity.OAuth.Token
{
    public class UnauthorizedClientResult : ITokenResult
    {
        public UnauthorizedClientResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.UnauthorizedClient, string.Empty, string.Empty);
        }

        public TokenErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
