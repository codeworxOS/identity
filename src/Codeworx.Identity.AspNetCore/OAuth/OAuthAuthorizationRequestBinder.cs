using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Binding.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class OAuthAuthorizationRequestBinder : AuthorizationRequestBinder<OAuthAuthorizationRequest, AuthorizationErrorResponse>
    {
        protected override IRequestBindingResult<OAuthAuthorizationRequest, AuthorizationErrorResponse> GetErrorResult(string errorDescription, string state)
        {
            return new OAuthErrorResult(errorDescription, state);
        }

        protected override IRequestBindingResult<OAuthAuthorizationRequest, AuthorizationErrorResponse> GetSuccessfulResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null)
        {
            var request = new OAuthAuthorizationRequest(clientId, redirectUri, responseType, scope, state, nonce, responseMode);

            return new OAuthSuccessfulBindingResult(request);
        }
    }
}