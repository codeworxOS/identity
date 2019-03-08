using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IOAuthClientService _oAuthClientService;
        private readonly IScopeService _scopeService;

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Code;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator authorizationCodeGenerator, IOAuthClientService oAuthClientService, IScopeService scopeService)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, IUser user, string currentTenantIdentifier)
        {
            var clientRegistrations = await _oAuthClientService.GetForTenantByIdentifier(currentTenantIdentifier);

            if (!clientRegistrations.Any(p => p.Identifier == request.ClientId && p.SupportedOAuthMode == request.ResponseType))
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

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request, user)
                                                                     .ConfigureAwait(false);

            return new SuccessfulAuthorizationResult(request.State, authorizationCode, request.RedirectUri);
        }
    }
}
