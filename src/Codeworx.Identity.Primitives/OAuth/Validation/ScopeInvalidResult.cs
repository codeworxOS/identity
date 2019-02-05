using System;

namespace Codeworx.Identity.OAuth.Validation
{
    public class ScopeInvalidResult : ValidationResult<AuthorizationErrorResponse>
    {
        public override AuthorizationErrorResponse GetError()
        {
            throw new NotImplementedException();
        }
    }
}