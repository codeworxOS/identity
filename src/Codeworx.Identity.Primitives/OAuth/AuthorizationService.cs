using System;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> _requestValidator;

        public AuthorizationService(IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> requestValidator)
        {
            _requestValidator = requestValidator;
        }

        public async Task<AuthorizationResult> AuthorizeRequest(AuthorizationRequest request)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            var validationError = _requestValidator.IsValid(request);
            if (validationError != null)
            {
                return new AuthorizationResult(validationError.Error);
            }

            await Task.Yield();

            return new AuthorizationResult(new AuthorizationCodeResponse(request.State, "HARDCODED TEST CODE"));
        }
    }
}
