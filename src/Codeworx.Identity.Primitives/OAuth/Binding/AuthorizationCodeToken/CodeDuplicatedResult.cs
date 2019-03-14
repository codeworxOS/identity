namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class CodeDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, TokenErrorResponse>
    {
        public CodeDuplicatedResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public TokenErrorResponse Error { get; }
    }
}
