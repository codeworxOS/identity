using System;

namespace Codeworx.Identity.OAuth.Validation
{
    public class ResponseTypeInvalidResult : ValidationResult<AuthorizationErrorResponse>
    {
        public override AuthorizationErrorResponse GetError()
        {
            throw new NotImplementedException();
        }
    }
}