namespace Codeworx.Identity.OAuth.Authorization
{
    public class UnsupportedResponseTypeResult : IAuthorizationResult
    {
        public UnsupportedResponseTypeResult(string state, string redirectUri)
        {
            this.Response = new AuthorizationErrorResponse(Constants.OAuth.Error.UnsupportedResponseType, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}