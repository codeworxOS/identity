namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class GrantTypeInvalidResult : IValidationResult<TokenErrorResponse>
    {
        public GrantTypeInvalidResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public TokenErrorResponse Error { get; }
    }
}
