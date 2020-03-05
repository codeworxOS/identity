namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class ClientIdDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, TokenErrorResponse>
    {
        public ClientIdDuplicatedResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public TokenErrorResponse Error { get; }
    }
}
