using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Account
{
    public class InvalidConfirmationCodeException : Exception
    {
        public InvalidConfirmationCodeException()
            : this("Invalid confirmation code provided!")
        {
        }

        protected InvalidConfirmationCodeException(string message)
            : base(message)
        {
        }

        protected InvalidConfirmationCodeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidConfirmationCodeException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
