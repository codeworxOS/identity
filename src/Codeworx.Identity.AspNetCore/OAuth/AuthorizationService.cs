using System;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> _requestValidator;
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;

        public AuthorizationService(IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> requestValidator, IAuthorizationCodeGenerator authorizationCodeGenerator)
        {
            _requestValidator = requestValidator;
            _authorizationCodeGenerator = authorizationCodeGenerator;
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

            var authorizationCodeGenerationResult = await _authorizationCodeGenerator.GenerateCode(request, userIdentifier)
                                                                     .ConfigureAwait(false);

            if (authorizationCodeGenerationResult.Error != null)
            {
                return new AuthorizationResult(authorizationCodeGenerationResult.Error);
            }

            return new AuthorizationResult(new AuthorizationCodeResponse(request.State, authorizationCodeGenerationResult.AuthorizationCode, request.RedirectUri));
        }
    }
}
