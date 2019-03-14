namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class GrantTypeDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, TokenErrorResponse>
    {
        public GrantTypeDuplicatedResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public TokenErrorResponse Error { get; }
    }
}
