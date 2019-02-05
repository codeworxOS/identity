using System;

namespace Codeworx.Identity.OAuth.Exceptions
{
    public abstract class AuthorizationRequestInvalidException : Exception
    {
        protected AuthorizationRequestInvalidException(string state)
        {
            this.State = state;
        }

        protected string State { get; }

        public abstract AuthorizationErrorResponse GetError();
    }
}
