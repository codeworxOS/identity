using System;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class InvalidRequestResult : IAuthorizationResult
    {
        private readonly IValidationResult<AuthorizationErrorResponse> _validationResult;

        public InvalidRequestResult(IValidationResult<AuthorizationErrorResponse> validationResult)
        {
            _validationResult = validationResult ?? throw new ArgumentNullException(nameof(validationResult));
        }

        public AuthorizationErrorResponse Error => _validationResult.Error;

        public AuthorizationCodeResponse Response => null;
    }
}
