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

        protected override IRequestBindingResult<OAuthAuthorizationRequest, AuthorizationErrorResponse> GetSuccessfulResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce)
        {
            var request = new OAuthAuthorizationRequest(clientId, redirectUri, responseType, scope, state, nonce);

            return new OAuthSuccessfulBindingResult(request);
        }
    }
}