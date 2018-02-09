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

        public AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}