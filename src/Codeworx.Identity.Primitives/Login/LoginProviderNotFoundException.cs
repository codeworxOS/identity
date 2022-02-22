using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Login
{
    public class LoginProviderNotFoundException : Exception, IEndUserErrorMessage
    {
        public LoginProviderNotFoundException(string providerId, string message)
            : base(message)
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

        public string GetMessage()
        {
            return this.Message;
        }
    }
}
