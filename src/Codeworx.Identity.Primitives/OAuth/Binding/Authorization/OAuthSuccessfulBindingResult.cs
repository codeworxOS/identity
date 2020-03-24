namespace Codeworx.Identity.OAuth.Binding.Authorization
{
    public class OAuthSuccessfulBindingResult : IRequestBindingResult<OAuthAuthorizationRequest, AuthorizationErrorResponse>
    {
        public OAuthSuccessfulBindingResult(OAuthAuthorizationRequest result)
        {
            this.Result = result;
        }

        public OAuthAuthorizationRequest Result { get; }

        public AuthorizationErrorResponse Error => null;
    }
}