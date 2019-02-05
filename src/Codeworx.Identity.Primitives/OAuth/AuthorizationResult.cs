namespace Codeworx.Identity.OAuth
{
    public class AuthorizationResult
    {
        public AuthorizationResult(AuthorizationErrorResponse error)
        {
            this.Error = error;
        }

        public AuthorizationResult(AuthorizationCodeResponse response)
        {
            this.Response = response;
        }

        public AuthorizationErrorResponse Error { get; }

        public AuthorizationCodeResponse Response { get; }
    }
}