namespace Codeworx.Identity.OAuth.Validation.Authorization
{
    public class RedirectUriInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public RedirectUriInvalidResult(string state = null)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, Constants.RedirectUriName, null, state);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}