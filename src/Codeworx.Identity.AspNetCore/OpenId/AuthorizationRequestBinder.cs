using Codeworx.Identity.OpenId;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class AuthorizationRequestBinder : AuthorizationRequestBinder<AuthorizationRequest>
    {
        protected override ErrorResponseException GetErrorResponse(string errorDescription, string state)
        {
            return new ErrorResponseException<Identity.OAuth.AuthorizationErrorResponse>(new Identity.OAuth.AuthorizationErrorResponse(Constants.OAuth.Error.InvalidRequest, errorDescription, null, state));
        }

        protected override AuthorizationRequest GetResult(string clientId, string redirectUri, string responseType, string scope, string state, string nonce = null, string responseMode = null)
        {
            return new AuthorizationRequest(clientId, redirectUri, responseType, scope, state, nonce, responseMode);
        }
    }
}