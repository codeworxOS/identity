namespace Codeworx.Identity.OAuth.BindingResults
{
    public class ClientIdDuplicatedResult : IRequestBindingResult<AuthorizationRequest, AuthorizationErrorResponse>
    {
        public ClientIdDuplicatedResult(string state)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, Constants.ClientIdName, null, state);
        }

        public AuthorizationRequest Result => null;

        public AuthorizationErrorResponse Error { get; }
    }
}