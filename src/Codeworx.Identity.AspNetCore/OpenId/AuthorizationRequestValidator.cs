using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class AuthorizationRequestValidator : AuthorizationRequestValidator<Identity.OpenId.AuthorizationRequest, AuthorizationErrorResponse>
    {
        public AuthorizationRequestValidator(IClientService clientService)
            : base(clientService)
        {
        }

        protected override IValidationResult<AuthorizationErrorResponse> GetInvalidResult(string errorDescription, string state, string redirectUri = null, string error = Identity.OAuth.Constants.Error.InvalidRequest)
        {
            return new Identity.OpenId.Validation.Authorization.InvalidResult(error, errorDescription, state, redirectUri);
        }
    }
}