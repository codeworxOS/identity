using System;

namespace Codeworx.Identity.OAuth.Validation
{
    public class RedirectUriInvalidResult : ValidationResult<AuthorizationErrorResponse>
    {
        public override AuthorizationErrorResponse GetError()
        {
            throw new NotImplementedException();
        }
    }
}