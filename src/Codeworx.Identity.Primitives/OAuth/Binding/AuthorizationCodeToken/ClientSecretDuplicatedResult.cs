namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class ClientSecretDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, TokenErrorResponse>
    {
        public ClientSecretDuplicatedResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public TokenErrorResponse Error { get; }
    }
}
