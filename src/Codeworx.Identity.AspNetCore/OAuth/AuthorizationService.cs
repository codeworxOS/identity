using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> _requestValidator;
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IUserService _userService;

        public AuthorizationService(IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> requestValidator, IAuthorizationCodeGenerator authorizationCodeGenerator, IUserService userService)
        {
            _requestValidator = requestValidator;
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _userService = userService;
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

            var user = await _userService.GetUserByIdentifierAsync(userIdentifier)
                                         .ConfigureAwait(false);

            if (user == null)
            {
                return new AuthorizationResult(new AuthorizationErrorResponse(Identity.OAuth.Constants.Error.AccessDenied, "", "", request.State, request.RedirectUri));
            }

            if (!user.OAuthClientRegistrations.Any(p => p.Identifier == request.ClientId && p.SupportedOAuthMode == request.ResponseType))
            {
                return new AuthorizationResult(new AuthorizationErrorResponse(Identity.OAuth.Constants.Error.UnauthorizedClient, string.Empty, string.Empty, request.State, request.RedirectUri));
            }

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request, user)
                                                                     .ConfigureAwait(false);

            return new AuthorizationResult(new AuthorizationCodeResponse(request.State, authorizationCode, request.RedirectUri));
        }
    }
}
