namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class ClientSecretInvalidResult : IValidationResult<ErrorResponse>
    {
        public ClientSecretInvalidResult()
        {
            this.Error = new ErrorResponse(Constants.OAuth.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }
    }
}
