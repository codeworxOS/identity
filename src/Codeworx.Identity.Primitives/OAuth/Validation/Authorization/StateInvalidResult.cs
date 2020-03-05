namespace Codeworx.Identity.OAuth.Validation.Authorization
{
    public class StateInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public StateInvalidResult(string redirectUri, string state = null)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, Constants.StateName, null, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}