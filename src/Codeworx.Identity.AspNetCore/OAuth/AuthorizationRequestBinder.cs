using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Binding.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationRequestBinder : AuthorizationRequestBinder<AuthorizationRequest, AuthorizationErrorResponse>
    {
        protected override IRequestBindingResult<AuthorizationRequest, AuthorizationErrorResponse> GetErrorResult(string errorDescription, string state)
        {
            return new ErrorResult(errorDescription, state);
        }

        protected override IRequestBindingResult<AuthorizationRequest, AuthorizationErrorResponse> GetSuccessfulResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null)
        {
            var request = new AuthorizationRequest(clientId, redirectUri, responseType, scope, state, nonce, responseMode);

            return new SuccessfulBindingResult(request);
        }
    }
}