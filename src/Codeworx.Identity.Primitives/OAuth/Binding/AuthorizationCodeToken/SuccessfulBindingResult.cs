namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class SuccessfulBindingResult : IRequestBindingResult<AuthorizationCodeTokenRequest, TokenErrorResponse>
    {
        public SuccessfulBindingResult(AuthorizationCodeTokenRequest result)
        {
            this.Result = result;
        }

        public AuthorizationCodeTokenRequest Result { get; }

        public TokenErrorResponse Error => null;
    }
}
