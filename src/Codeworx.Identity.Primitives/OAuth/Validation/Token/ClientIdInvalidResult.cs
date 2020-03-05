namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class ClientIdInvalidResult : IValidationResult<TokenErrorResponse>
    {
        public ClientIdInvalidResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public TokenErrorResponse Error { get; }
    }
}
