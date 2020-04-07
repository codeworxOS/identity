using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class AuthorizationRequestBinder : AuthorizationRequestBinder<Identity.OpenId.AuthorizationRequest, AuthorizationErrorResponse>
    {
        protected override IRequestBindingResult<Identity.OpenId.AuthorizationRequest, AuthorizationErrorResponse> GetErrorResult(string errorDescription, string state)
        {
            return new Identity.OpenId.Binding.Authorization.ErrorResult(errorDescription, state);
        }

        protected override IRequestBindingResult<Identity.OpenId.AuthorizationRequest, AuthorizationErrorResponse> GetSuccessfulResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null)
        {
            var request = new Identity.OpenId.AuthorizationRequest(clientId, redirectUri, responseType, scope, state, nonce, responseMode);

            return new Identity.OpenId.Binding.Authorization.SuccessfulBindingResult(request);
        }
    }
}