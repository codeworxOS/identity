namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class RedirectUriInvalidResult : IValidationResult<ErrorResponse>
    {
        public RedirectUriInvalidResult()
        {
            this.Error = new ErrorResponse(Constants.OAuth.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }
    }
}
