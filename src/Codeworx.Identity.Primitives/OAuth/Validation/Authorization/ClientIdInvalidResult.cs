namespace Codeworx.Identity.OAuth.Validation.Authorization
{
    public class ClientIdInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public ClientIdInvalidResult(string state = null)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, Constants.ClientIdName, null, state);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}
