using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Validation.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationRequestValidator : AuthorizationRequestValidator<AuthorizationRequest, AuthorizationErrorResponse>
    {
        public AuthorizationRequestValidator(IClientService clientService)
            : base(clientService)
        {
        }

        protected override IValidationResult<AuthorizationErrorResponse> GetInvalidResult(string errorDescription, string state, string redirectUri = null, string error = Identity.OAuth.Constants.Error.InvalidRequest)
        {
            return new InvalidResult(error, errorDescription, state, redirectUri);
        }
    }
}