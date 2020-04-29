using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IEnumerable<IAuthorizationRequestProcessor> _requestProcessors;
        private readonly IEnumerable<IAuthorizationFlowService> _authorizationFlowServices;
        private readonly IUserService _userService;

        public AuthorizationService(
            IEnumerable<IAuthorizationRequestProcessor> requestProcessors,
            IEnumerable<IAuthorizationFlowService> authorizationFlowServices,
            IUserService userService)
        {
            _requestProcessors = requestProcessors;
            _authorizationFlowServices = authorizationFlowServices;
            _userService = userService;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, ClaimsIdentity user)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, user);

            foreach (var processor in _requestProcessors)
            {
                builder = await processor.ProcessAsync(builder, request).ConfigureAwait(false);
            }

            var parameters = builder.Parameters;

            var currentUser = await _userService.GetUserByIdentifierAsync(parameters.User)
                                         .ConfigureAwait(false);

            if (currentUser == null)
            {
                return new UserNotFoundResult(parameters.State, parameters.RedirectUri);
            }

            var authorizationFlowService = _authorizationFlowServices.FirstOrDefault(p => p.IsSupported(parameters.ResponseTypes.First()));
            if (authorizationFlowService == null)
            {
                return new UnsupportedResponseTypeResult(parameters.State, parameters.RedirectUri);
            }

            var authorizationResult = await authorizationFlowService.AuthorizeRequest(parameters)
                                                                    .ConfigureAwait(false);

            return authorizationResult;
        }
    }
}