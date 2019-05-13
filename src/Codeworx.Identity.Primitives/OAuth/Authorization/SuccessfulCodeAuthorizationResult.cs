namespace Codeworx.Identity.OAuth.Authorization
{
    public class SuccessfulCodeAuthorizationResult : IAuthorizationResult
    {
        public SuccessfulCodeAuthorizationResult(string state, string authorizationCode, string redirectUri)
        {
            this.Response = new AuthorizationCodeResponse(state, authorizationCode, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}