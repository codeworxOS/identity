namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class ClientSecretInvalidResult : IValidationResult<ErrorResponse>
    {
        public ClientSecretInvalidResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }
    }
}
