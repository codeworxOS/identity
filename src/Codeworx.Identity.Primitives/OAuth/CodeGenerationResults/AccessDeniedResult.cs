namespace Codeworx.Identity.OAuth.CodeGenerationResults
{
    public class AccessDeniedResult : IAuthorizationCodeGenerationResult
    {
        public AccessDeniedResult(string redirectUri, string state)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.AccessDenied, string.Empty, string.Empty, state, redirectUri);
        }

        public string AuthorizationCode => null;

        public AuthorizationErrorResponse Error { get; }
    }
}
