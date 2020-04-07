using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationService<TRequest> : IAuthorizationService<TRequest>
        where TRequest : AuthorizationRequest
    {
        private readonly IEnumerable<IAuthorizationFlowService<TRequest>> _authorizationFlowServices;
        private readonly IRequestValidator<TRequest, AuthorizationErrorResponse> _requestValidator;
        private readonly IUserService _userService;

        public AuthorizationService(IRequestValidator<TRequest, AuthorizationErrorResponse> requestValidator, IEnumerable<IAuthorizationFlowService<TRequest>> authorizationFlowServices, IUserService userService)
        {
            _requestValidator = requestValidator;
            _authorizationFlowServices = authorizationFlowServices;
            _userService = userService;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(TRequest request, ClaimsIdentity user)
        {
            if (request == null)
            {
                throw new ArgumentNullException();
            }

            var validationError = await _requestValidator.IsValid(request)
                                                         .ConfigureAwait(false);
            if (validationError != null)
            {
                return new InvalidRequestResult(validationError);
            }

            var currentUser = await _userService.GetUserByIdentifierAsync(user)
                                         .ConfigureAwait(false);

            if (currentUser == null)
            {
                return new UserNotFoundResult(request.State, request.RedirectionTarget);
            }

            var authorizationFlowService = _authorizationFlowServices.FirstOrDefault(p => p.IsSupported(request.ResponseType));
            if (authorizationFlowService == null)
            {
                return new UnsupportedResponseTypeResult(request.State, request.RedirectionTarget);
            }

            var authorizationResult = await authorizationFlowService.AuthorizeRequest(request, user)
                                                                    .ConfigureAwait(false);

            return authorizationResult;
        }
    }
}