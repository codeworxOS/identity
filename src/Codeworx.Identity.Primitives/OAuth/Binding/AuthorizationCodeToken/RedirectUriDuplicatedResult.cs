namespace Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken
{
    public class RedirectUriDuplicatedResult : IRequestBindingResult<AuthorizationCodeTokenRequest, ErrorResponse>
    {
        public RedirectUriDuplicatedResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, string.Empty, string.Empty);
        }

        public AuthorizationCodeTokenRequest Result => null;

        public ErrorResponse Error { get; }
    }
}
