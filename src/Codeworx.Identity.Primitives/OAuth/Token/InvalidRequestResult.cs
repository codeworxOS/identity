using System;

namespace Codeworx.Identity.OAuth.Token
{
    public class InvalidRequestResult : ITokenResult
    {
        public InvalidRequestResult(IValidationResult<ErrorResponse> validationResult)
        {
            this.Error = validationResult.Error ?? throw new ArgumentNullException(nameof(validationResult));
        }

        public InvalidRequestResult()
        {
            this.Error = new ErrorResponse(Constants.Error.InvalidRequest, null, null);
        }

        public ErrorResponse Error { get; }

        public TokenResponse Response => null;
    }
}
