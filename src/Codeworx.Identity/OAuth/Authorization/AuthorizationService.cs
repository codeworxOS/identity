using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationService : IAuthorizationService
    {
        private readonly IEnumerable<IAuthorizationRequestProcessor> _requestProcessors;
        private readonly IEnumerable<IAuthorizationResponseProcessor> _responseProcessors;
        private readonly IUserService _userService;

        public AuthorizationService(
            IEnumerable<IAuthorizationRequestProcessor> requestProcessors,
            IEnumerable<IAuthorizationResponseProcessor> responseProcessors,
            IUserService userService)
        {
            _requestProcessors = requestProcessors;
            _responseProcessors = responseProcessors;
            _userService = userService;
        }

        public async Task<AuthorizationSuccessResponse> AuthorizeRequest(AuthorizationRequest request, ClaimsIdentity user)
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

            IAuthorizationResponseBuilder responseBuilder = null;
            responseBuilder.WithState(parameters.State)
                .WithRedirectUri(parameters.RedirectUri);

            var currentUser = await _userService.GetUserByIdentifierAsync(parameters.User)
                                         .ConfigureAwait(false);

            if (currentUser == null)
            {
                responseBuilder.RaiseError(Constants.OAuth.Error.AccessDenied);
            }

            foreach (var item in _responseProcessors)
            {
                responseBuilder = await item.ProcessAsync(parameters, , responseBuilder);
            }

            return responseBuilder.Response;
        }
    }
}