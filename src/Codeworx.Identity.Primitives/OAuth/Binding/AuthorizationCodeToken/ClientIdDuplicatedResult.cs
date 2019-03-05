namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class ClientIdDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, object>
    {
        public AuthorizationCodeTokenRequest Result => null;

        public object Error { get; }
    }
}
