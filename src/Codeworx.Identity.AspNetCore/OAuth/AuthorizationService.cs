using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> _requestValidator;
        private readonly IEnumerable<IAuthorizationFlowService> _authorizationFlowServices;
        private readonly IUserService _userService;

        public AuthorizationService(IRequestValidator<AuthorizationRequest, AuthorizationErrorResponse> requestValidator, IEnumerable<IAuthorizationFlowService> authorizationFlowServices, IUserService userService)
        {
            _requestValidator = requestValidator;
            _authorizationFlowServices = authorizationFlowServices;
            _userService = userService;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, string userIdentifier)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            var validationError = _requestValidator.IsValid(request);
            if (validationError != null)
            {
                return new InvalidRequestResult(validationError);
            }

            var user = await _userService.GetUserByIdentifierAsync(userIdentifier)
                                         .ConfigureAwait(false);

            if (user == null)
            {
                return new UserNotFoundResult(request.State, request.RedirectUri);
            }

            var authorizationFlowService = _authorizationFlowServices.FirstOrDefault(p => p.SupportedAuthorizationResponseType == request.ResponseType);
            if (authorizationFlowService == null)
            {
                return new UnsupportedResponseTypeResult(request.State, request.RedirectUri);
            }

            var authorizationResult = await authorizationFlowService.AuthorizeRequest(request)
                                                                    .ConfigureAwait(false);
            
            return authorizationResult;
        }
    }
}
