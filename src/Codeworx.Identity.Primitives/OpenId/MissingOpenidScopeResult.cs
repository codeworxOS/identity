using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.OpenId
{
    public class MissingOpenidScopeResult : IAuthorizationResult
    {
        public MissingOpenidScopeResult(string state, string redirectUri)
        {
            this.Response = new AuthorizationErrorResponse(Constants.OAuth.Error.InvalidScope, string.Empty, string.Empty, state, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}