using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationService<TRequest> : IAuthorizationService<TRequest>
        where TRequest : AuthorizationRequest
    {
        private readonly IEnumerable<IIdentityRequestProcessor<IAuthorizationParameters, TRequest>> _requestProcessors;
        private readonly IEnumerable<IAuthorizationResponseProcessor> _responseProcessors;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public AuthorizationService(
            IEnumerable<IIdentityRequestProcessor<IAuthorizationParameters, TRequest>> requestProcessors,
            IEnumerable<IAuthorizationResponseProcessor> responseProcessors,
            IUserService userService,
            IIdentityService identityService)
        {
            _requestProcessors = requestProcessors;
            _responseProcessors = responseProcessors;
            _userService = userService;
            _identityService = identityService;
        }

        public async Task<AuthorizationSuccessResponse> AuthorizeRequest(TRequest request, ClaimsIdentity user)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            IAuthorizationParametersBuilder builder = new AuthorizationParametersBuilder(request, user);

            foreach (var processor in _requestProcessors.OrderBy(p => p.SortOrder))
            {
                await processor.ProcessAsync(builder, request).ConfigureAwait(false);
            }

            var parameters = builder.Parameters;

            IAuthorizationResponseBuilder responseBuilder = new AuthorizationResponseBuilder();
            responseBuilder.WithState(parameters.State)
                .WithResponseMode(parameters.ResponseMode)
                .WithRedirectUri(parameters.RedirectUri);

            var currentUser = await _userService.GetUserByIdentityAsync(parameters.User)
                                         .ConfigureAwait(false);

            if (currentUser == null)
            {
                responseBuilder.RaiseError(Constants.OAuth.Error.AccessDenied);
            }

            var data = await _identityService.GetIdentityAsync(parameters).ConfigureAwait(false);

            foreach (var item in _responseProcessors)
            {
                responseBuilder = await item.ProcessAsync(parameters, data, responseBuilder).ConfigureAwait(false);
            }

            return responseBuilder.Response;
        }
    }
}