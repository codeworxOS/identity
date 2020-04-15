namespace Codeworx.Identity.OAuth.Binding.Authorization
{
    public class SuccessfulBindingResult : IRequestBindingResult<AuthorizationRequest, AuthorizationErrorResponse>
    {
        public SuccessfulBindingResult(AuthorizationRequest result)
        {
            this.Result = result;
        }

        public AuthorizationRequest Result { get; }

        public AuthorizationErrorResponse Error => null;
    }
}