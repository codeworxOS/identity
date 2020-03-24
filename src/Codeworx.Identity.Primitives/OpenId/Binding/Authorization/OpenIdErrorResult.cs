using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.OpenId.Binding.Authorization
{
    public class OpenIdErrorResult : IRequestBindingResult<OpenIdAuthorizationRequest, AuthorizationErrorResponse>
    {
        public OpenIdErrorResult(string errorDescription, string state)
        {
            this.Error = new AuthorizationErrorResponse(OAuth.Constants.Error.InvalidRequest, errorDescription, null, state);
        }

        public OpenIdAuthorizationRequest Result => null;

        public AuthorizationErrorResponse Error { get; }
    }
}