namespace Codeworx.Identity.OAuth.Authorization
{
    public class UnauthorizedClientResult : IAuthorizationResult
    {
        public UnauthorizedClientResult(string state, string redirectUri)
        {
            this.Response = new AuthorizationErrorResponse(Constants.Error.UnauthorizedClient, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}