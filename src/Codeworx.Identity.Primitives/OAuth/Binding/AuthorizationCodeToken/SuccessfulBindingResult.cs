namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class SuccessfulBindingResult : IRequestBindingResult<AuthorizationCodeTokenRequest, object>
    {
        public SuccessfulBindingResult(AuthorizationCodeTokenRequest result)
        {
            this.Result = result;
        }

        public AuthorizationCodeTokenRequest Result { get; }

        public object Error => null;
    }
}
