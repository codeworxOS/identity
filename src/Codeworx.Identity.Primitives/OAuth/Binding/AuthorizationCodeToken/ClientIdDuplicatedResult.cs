namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class ClientIdDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, ErrorResponse>
    {
        public ClientIdDuplicatedResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public ErrorResponse Error { get; }
    }
}
