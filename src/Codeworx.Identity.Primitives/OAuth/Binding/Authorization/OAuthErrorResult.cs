namespace Codeworx.Identity.OAuth.Binding.Authorization
{
    public class OAuthErrorResult : IRequestBindingResult<OAuthAuthorizationRequest, AuthorizationErrorResponse>
    {
        public OAuthErrorResult(string errorDescription, string state)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.InvalidRequest, errorDescription, null, state);
        }

        public OAuthAuthorizationRequest Result => null;

        public AuthorizationErrorResponse Error { get; }
    }
}
