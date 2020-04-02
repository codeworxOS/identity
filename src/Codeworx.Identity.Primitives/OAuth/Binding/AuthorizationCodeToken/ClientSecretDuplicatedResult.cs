namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class ClientSecretDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, ErrorResponse>
    {
        public ClientSecretDuplicatedResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public ErrorResponse Error { get; }
    }
}
