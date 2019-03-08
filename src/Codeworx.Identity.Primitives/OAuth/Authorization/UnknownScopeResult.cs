namespace Codeworx.Identity.OAuth.Authorization
{
    public class UnknownScopeResult : IAuthorizationResult
    {
        public UnknownScopeResult(string state, string redirectUri)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidScope, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }

        public AuthorizationCodeResponse Response => null;
    }
}
