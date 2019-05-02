namespace Codeworx.Identity.OAuth.Authorization
{
    public class SuccessfulAuthorizationResult : IAuthorizationResult
    {
        public SuccessfulAuthorizationResult(string state, string authorizationCode, string redirectUri)
        {
            this.Response = new AuthorizationCodeResponse(state, authorizationCode, redirectUri);
        }

        public AuthorizationErrorResponse Error => null;

        public AuthorizationCodeResponse Response { get; }
    }
}
