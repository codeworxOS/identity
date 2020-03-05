namespace Codeworx.Identity.OAuth.Authorization
{
    public class UserNotFoundResult : IAuthorizationResult
    {
        public UserNotFoundResult(string state, string redirectUri = null)
        {
            this.Response = new AuthorizationErrorResponse(Constants.Error.AccessDenied, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}