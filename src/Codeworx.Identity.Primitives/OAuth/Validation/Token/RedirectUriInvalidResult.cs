namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class RedirectUriInvalidResult : IValidationResult<ErrorResponse>
    {
        public RedirectUriInvalidResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }
    }
}
