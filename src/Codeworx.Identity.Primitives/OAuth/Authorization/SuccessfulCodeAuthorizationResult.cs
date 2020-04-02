namespace Codeworx.Identity.OAuth.Authorization
{
    public class SuccessfulCodeAuthorizationResult : IAuthorizationResult
    {
        public SuccessfulCodeAuthorizationResult(string state, string authorizationCode, string redirectUri, string responseMode = null)
        {
            this.Response = new AuthorizationCodeResponse(state, authorizationCode, redirectUri, responseMode);
        }

        public AuthorizationResponse Response { get; }
    }
}