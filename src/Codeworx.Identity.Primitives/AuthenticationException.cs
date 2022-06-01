using System;

namespace Codeworx.Identity
{
    public class AuthenticationException : Exception, IEndUserErrorMessage
    {
        public AuthenticationException(string message)
            : base(message)
        {
        }

        protected AuthenticationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        public string GetMessage()
        {
            return Message;
        }
    }
}