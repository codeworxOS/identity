namespace Codeworx.Identity.OAuth.Validation.Token
{
    public class RedirectUriInvalidResult : IValidationResult<TokenErrorResponse>
    {
        public RedirectUriInvalidResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public TokenErrorResponse Error { get; }
    }
}
