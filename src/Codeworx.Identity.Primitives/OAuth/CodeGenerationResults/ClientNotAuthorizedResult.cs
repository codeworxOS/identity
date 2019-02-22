namespace Codeworx.Identity.OAuth.CodeGenerationResults
{
    public class ClientNotAuthorizedResult : IAuthorizationCodeGenerationResult
    {
        public ClientNotAuthorizedResult(string redirectUri, string state)
        {
            this.Error = new AuthorizationErrorResponse(Constants.Error.UnauthorizedClient, string.Empty, string.Empty, state, redirectUri);
        }

        public string AuthorizationCode => null;

        public AuthorizationErrorResponse Error { get; }
    }
}
