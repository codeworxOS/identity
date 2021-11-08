using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationService<TRequest> : IAuthorizationService<TRequest>
        where TRequest : AuthorizationRequest
    {
        private readonly IReadOnlyList<IIdentityRequestProcessor<IAuthorizationParameters, TRequest>> _requestProcessors;
        private readonly IReadOnlyList<IAuthorizationResponseProcessor> _responseProcessors;
        private readonly IUserService _userService;
        private readonly IIdentityService _identityService;

        public AuthorizationService(
            IEnumerable<IIdentityRequestProcessor<IAuthorizationParameters, TRequest>> requestProcessors,
            IEnumerable<IAuthorizationResponseProcessor> responseProcessors,
            IUserService userService,
            IIdentityService identityService)
        {
            _requestProcessors = requestProcessors.OrderBy(p => p.SortOrder).ToImmutableList();
            _responseProcessors = responseProcessors.ToImmutableList();
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

            foreach (var processor in _requestProcessors)
            {
                await processor.ProcessAsync(builder, request).ConfigureAwait(false);
            }

            var parameters = builder.Parameters;

            IAuthorizationResponseBuilder responseBuilder = new AuthorizationResponseBuilder();
            responseBuilder.WithState(parameters.State)
                .WithResponseMode(parameters.ResponseMode)
                .WithRedirectUri(parameters.RedirectUri);

            var currentUser = await _userService.GetUserByIdentifierAsync(parameters.User)
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