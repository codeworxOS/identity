namespace Codeworx.Identity.OAuth.Authorization
{
    public class UserNotFoundResult : IAuthorizationResult
    {
        public UserNotFoundResult(string state, string redirectUri = null)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.AccessDenied, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }

        public AuthorizationCodeResponse Response => null;
    }
}
