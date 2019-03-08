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

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Code;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator authorizationCodeGenerator, IOAuthClientService oAuthClientService)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _oAuthClientService = oAuthClientService;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, IUser user)
        {
            var clientRegistrations = await _oAuthClientService.GetForTenantByIdentifier(user.DefaultTenantKey);

            if (!clientRegistrations.Any(p => p.Identifier == request.ClientId && p.SupportedOAuthMode == request.ResponseType))
            {
                return new UnauthorizedClientResult(request.State, request.RedirectUri);
            }

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request, user)
                                                                     .ConfigureAwait(false);

            return new SuccessfulAuthorizationResult(request.State, authorizationCode, request.RedirectUri);
        }
    }
}
