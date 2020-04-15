namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class GrantTypeInvalidResult : IValidationResult<ErrorResponse>
    {
        public GrantTypeInvalidResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }
    }
}
