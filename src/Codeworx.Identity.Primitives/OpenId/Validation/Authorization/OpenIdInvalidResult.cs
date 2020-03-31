using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.OpenId.Validation.Authorization
{
    public class OpenIdInvalidResult : IValidationResult<AuthorizationErrorResponse>
    {
        public OpenIdInvalidResult(string error, string errorDescription, string state, string redirectUri = null)
        {
            this.Error = new AuthorizationErrorResponse(error, errorDescription, null, state, redirectUri);
        }

        public AuthorizationErrorResponse Error { get; }
    }
}