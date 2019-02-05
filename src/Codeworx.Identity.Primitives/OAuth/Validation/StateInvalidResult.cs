using System;

namespace Codeworx.Identity.OAuth.Validation
{
    public class StateInvalidResult : ValidationResult<AuthorizationErrorResponse>
    {
        public override AuthorizationErrorResponse GetError()
        {
            throw new NotImplementedException();
        }
    }
}