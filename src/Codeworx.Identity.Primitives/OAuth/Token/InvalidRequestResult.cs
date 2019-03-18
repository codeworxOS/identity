using System;

namespace Codeworx.Identity.OAuth.Token
{
    public class InvalidRequestResult : ITokenResult
    {
        private readonly IValidationResult<TokenErrorResponse> _validationResult;

        public InvalidRequestResult(IValidationResult<TokenErrorResponse> validationResult)
        {
            _validationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        public TokenErrorResponse Error => _validationResult.Error;

        public TokenResponse Response => null;
    }
}
