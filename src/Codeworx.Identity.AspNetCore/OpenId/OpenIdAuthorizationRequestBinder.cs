using Codeworx.Identity.OAuth;
using Codeworx.Identity.OpenId;
using Codeworx.Identity.OpenId.Binding.Authorization;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class OpenIdAuthorizationRequestBinder : AuthorizationRequestBinder<OpenIdAuthorizationRequest, AuthorizationErrorResponse>
    {
        protected override IRequestBindingResult<OpenIdAuthorizationRequest, AuthorizationErrorResponse> GetErrorResult(string errorDescription, string state)
        {
            return new OpenIdErrorResult(errorDescription, state);
        }

        protected override IRequestBindingResult<OpenIdAuthorizationRequest, AuthorizationErrorResponse> GetSuccessfulResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null)
        {
            var request = new OpenIdAuthorizationRequest(clientId, redirectUri, responseType, scope, state, nonce, responseMode);

            return new OpenIdSuccessfulBindingResult(request);
        }
    }
}