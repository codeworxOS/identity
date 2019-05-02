namespace Codeworx.Identity.OAuth.Authorization
{
    public class UnsupportedResponseTypeResult : IAuthorizationResult
    {
        public UnsupportedResponseTypeResult(string state, string redirectUri)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.UnsupportedResponseType, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }

        public AuthorizationCodeResponse Response => null;
    }
}
