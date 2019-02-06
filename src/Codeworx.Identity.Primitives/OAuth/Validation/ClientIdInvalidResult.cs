using System;

namespace Codeworx.Identity.OAuth.Validation
{
    public class ClientIdInvalidResult : ValidationResult<AuthorizationErrorResponse>
    {
        public override AuthorizationErrorResponse GetError()
        {
            throw new NotImplementedException();
        }
    }
}
