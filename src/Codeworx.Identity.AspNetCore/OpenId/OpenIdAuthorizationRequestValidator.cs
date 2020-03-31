using Codeworx.Identity.OAuth;
using Codeworx.Identity.OpenId;
using Codeworx.Identity.OpenId.Validation.Authorization;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class OpenIdAuthorizationRequestValidator : AuthorizationRequestValidator<OpenIdAuthorizationRequest, AuthorizationErrorResponse>
    {
        public OpenIdAuthorizationRequestValidator(IClientService clientService)
            : base(clientService)
        {
        }

        protected override IValidationResult<AuthorizationErrorResponse> GetInvalidResult(string errorDescription, string state, string redirectUri = null, string error = Identity.OAuth.Constants.Error.InvalidRequest)
        {
            return new OpenIdInvalidResult(error, errorDescription, state, redirectUri);
        }
    }
}