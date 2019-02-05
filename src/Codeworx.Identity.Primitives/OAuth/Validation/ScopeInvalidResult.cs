namespace Codeworx.Identity.OAuth.Validation
{
    public class ScopeInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public ScopeInvalidResult(string redirectUri, string state = null)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, Constants.ScopeName, null, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}