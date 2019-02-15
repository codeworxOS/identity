using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> _requestValidator;

        public AuthorizationService(IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> requestValidator)
        {
            _requestValidator = requestValidator;
        }

        public async Task<AuthorizationResult> AuthorizeRequest(AuthorizationRequest request, string userIdentifier)
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

            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                return new AuthorizationResult(new AuthorizationErrorResponse(Identity.OAuth.Constants.Error.AccessDenied, "", "", request.State, request.RedirectUri));
            }
            
            await Task.Yield();

            return new AuthorizationResult(new AuthorizationCodeResponse(request.State, "HARDCODED TEST CODE", request.RedirectUri));
        }
    }
}
