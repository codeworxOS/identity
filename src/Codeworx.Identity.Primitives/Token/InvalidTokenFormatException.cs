using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Token
{
    public class InvalidTokenFormatException : Exception
    {
        public InvalidTokenFormatException()
            : base($"The token format of the provided token is invalid. No token provider found.")
        {
        }

        protected InvalidTokenFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidTokenFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
