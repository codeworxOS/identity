namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class SuccessfulBindingResult : IRequestBindingResult<AuthorizationCodeTokenRequest, ErrorResponse>
    {
        public SuccessfulBindingResult(AuthorizationCodeTokenRequest result)
        {
            this.Result = result;
        }

        public AuthorizationCodeTokenRequest Result { get; }

        public ErrorResponse Error => null;
    }
}
