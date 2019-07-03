using System;

namespace Codeworx.Identity.OAuth.Token
{
    public class InvalidRequestResult : ITokenResult
    {
        public InvalidRequestResult(IValidationResult<TokenErrorResponse> validationResult)
        {
            this.Error = validationResult.Error ?? throw new ArgumentNullException(nameof(validationResult));
        }

        public InvalidRequestResult()
        {
            this.Error = new TokenErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public TokenErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
