namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class ClientSecretInvalidResult : IValidationResult<TokenErrorResponse>
    {
        public ClientSecretInvalidResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public TokenErrorResponse Error { get; }
    }
}
