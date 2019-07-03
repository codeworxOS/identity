namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class RedirectUriDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, TokenErrorResponse>
    {
        public RedirectUriDuplicatedResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public TokenErrorResponse Error { get; }
    }
}
