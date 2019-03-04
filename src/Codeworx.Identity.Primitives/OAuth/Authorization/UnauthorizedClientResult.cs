namespace Codeworx.Identity.OAuth.Authorization
{
    public class UnauthorizedClientResult : IAuthorizationResult
    {
        public UnauthorizedClientResult(string state, string redirectUri)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.UnauthorizedClient, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }

        public AuthorizationCodeResponse Response => null;
    }
}
