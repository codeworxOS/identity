using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Login
{
    public class LoginProviderNotFoundException : Exception
    {
        public LoginProviderNotFoundException(string providerId)
            : base($"Login provider {providerId} not found!")
        {
        }

        protected LoginProviderNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected LoginProviderNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
