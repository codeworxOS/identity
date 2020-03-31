namespace Codeworx.Identity.OAuth.Validation.Authorization
{
    public class OAuthInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public OAuthInvalidResult(string error, string errorDescription, string state, string redirectUri = null)
        {
            this.Error = new AuthorizationErrorResponse(error, errorDescription, null, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}