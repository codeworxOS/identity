namespace Codeworx.Identity.OAuth.Validation.Authorization
{
    public class ScopeInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public ScopeInvalidResult(string redirectUri, string state = null)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidScope, Constants.ScopeName, null, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}