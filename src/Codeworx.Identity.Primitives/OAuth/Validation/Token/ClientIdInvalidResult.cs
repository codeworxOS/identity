namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class ClientIdInvalidResult : IValidationResult<ErrorResponse>
    {
        public ClientIdInvalidResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }
    }
}
