namespace Codeworx.Identity.OAuth.Validation
{
    public class ResponseTypeInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public ResponseTypeInvalidResult(string redirectUri, string state = null)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, Constants.ResponseTypeName, null, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}