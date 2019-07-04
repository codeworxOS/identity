using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Validation.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationTokenFlowService : IAuthorizationFlowService
    {
        private readonly IClientService _oAuthClientService;
        private readonly IScopeService _scopeService;

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Token;

        public AuthorizationTokenFlowService(IClientService oAuthClientService, IScopeService scopeService)
        {
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request)
        {
            var client = await _oAuthClientService.GetById(request.ClientId);

            if (client == null)
            {
                return new InvalidRequestResult(new ClientIdInvalidResult(request.State));
            }

            // TODO check if response type is allowed
            ////if (clientRegistrations.Any(p => p.Identifier == request.ClientId && p.SupportedOAuthMode == request.ResponseType) == false)
            ////{
            ////    return new UnauthorizedClientResult(request.State, request.RedirectUri);
            ////}

            var scopes = await _scopeService.GetScopes()
                                            .ConfigureAwait(false);

            var scopeKeys = scopes
                            .Select(s => s.ScopeKey)
                            .ToList();

            if (!string.IsNullOrEmpty(request.Scope)
                && request.Scope
                          .Split(' ')
                          .Any(p => scopeKeys.Contains(p) == false))
            {
                return new UnknownScopeResult(request.State, request.RedirectUri);
            }

            return new SuccessfulTokenAuthorizationResult(request.State, "AuthorizationToken", request.RedirectUri);
        }
    }
}