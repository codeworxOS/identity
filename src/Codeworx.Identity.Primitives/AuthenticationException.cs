using System;

namespace Codeworx.Identity
{
    public class AuthenticationException : Exception
    {
        public AuthenticationException()
            : this(Constants.AuthenticationExceptionMessage)
        {
        }

        public AuthenticationException(string message)
            : base(message)
        {
        }

        protected AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}