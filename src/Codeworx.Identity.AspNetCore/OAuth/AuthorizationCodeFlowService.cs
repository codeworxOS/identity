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

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Code;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator authorizationCodeGenerator)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, IUser user)
        {
            if (!user.OAuthClientRegistrations.Any(p => p.Identifier == request.ClientId && p.SupportedOAuthMode == request.ResponseType))
            {
                return new UnauthorizedClientResult(request.State, request.RedirectUri);
            }

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request, user)
                                                                     .ConfigureAwait(false);

            return new SuccessfulAuthorizationResult(request.State, authorizationCode, request.RedirectUri);
        }
    }
}
