namespace Codeworx.Identity.OAuth.Binding.Authorization
{
    public class RedirectUriDuplicatedResult : IRequestBindingResult<AuthorizationRequest, AuthorizationErrorResponse>
    {
        public RedirectUriDuplicatedResult(string state)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, Constants.RedirectUriName, null, state);
        }

        public AuthorizationRequest Result => null;

        public AuthorizationErrorResponse Error { get; }
    }
}