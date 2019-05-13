using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationTokenFlowService: IAuthorizationFlowService
    {
        private readonly IOAuthClientService _oAuthClientService;
        private readonly IScopeService _scopeService;

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Token;

        public AuthorizationTokenFlowService(IOAuthClientService oAuthClientService, IScopeService scopeService)
        {
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, string currentTenantIdentifier)
        {
            var clientRegistrations = await _oAuthClientService.GetForTenantByIdentifier(currentTenantIdentifier);

            if (clientRegistrations.Any(p => p.Identifier == request.ClientId && p.SupportedOAuthMode == request.ResponseType) == false)
            {
                return new UnauthorizedClientResult(request.State, request.RedirectUri);
            }

            var scopes = await _scopeService.GetScopes()
                                            .ConfigureAwait(false);

            var scopeKeys = scopes
                            .Select(s => s.ScopeKey)
                            .ToList();

            if (!string.IsNullOrEmpty(request.Scope)
                && request.Scope
                          .Split(' ')
                          .Any(p => !scopeKeys.Contains(p)) == true)
            {
                return new UnknownScopeResult(request.State, request.RedirectUri);
            }

            throw new NotImplementedException();
        }
    }
}