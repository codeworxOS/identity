namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class ClientIdInvalidResult : IValidationResult<ErrorResponse>
    {
        public ClientIdInvalidResult()
        {
            this.Error = new ErrorResponse(Constants.OAuth.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }
    }
}
