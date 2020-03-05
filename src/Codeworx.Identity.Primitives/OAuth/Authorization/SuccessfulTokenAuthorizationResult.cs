namespace Codeworx.Identity.OAuth.Authorization
{
    public class SuccessfulTokenAuthorizationResult : IAuthorizationResult
    {
        public SuccessfulTokenAuthorizationResult(string state, string token, string redirectUri)
        {
            this.Response = new AuthorizationTokenResponse(state, token, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}