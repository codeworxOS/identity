namespace Codeworx.Identity.OAuth.Authorization
{
    public class SuccessfulTokenAuthorizationResult : IAuthorizationResult
    {
        public SuccessfulTokenAuthorizationResult(string state, string token, int expiresIn, string redirectUri)
        {
            this.Response = new AuthorizationTokenResponse(state, token, expiresIn, redirectUri);
        }

        public AuthorizationResponse Response { get; }
    }
}