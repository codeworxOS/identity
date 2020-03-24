using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.OpenId.Binding.Authorization
{
    public class OpenIdSuccessfulBindingResult : IRequestBindingResult<OpenIdAuthorizationRequest, AuthorizationErrorResponse>
    {
        public OpenIdSuccessfulBindingResult(OpenIdAuthorizationRequest result)
        {
            this.Result = result;
        }

        public OpenIdAuthorizationRequest Result { get; }

        public AuthorizationErrorResponse Error => null;
    }
}