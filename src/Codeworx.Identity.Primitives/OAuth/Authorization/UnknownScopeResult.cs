namespace Codeworx.Identity.OAuth.Authorization
{
    public class UnknownScopeResult : IAuthorizationResult
    {
        public UnknownScopeResult(string state, string redirectUri)
        {
            this.Response = new AuthorizationErrorResponse(Constants.Error.InvalidScope, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}