using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity
{
    public class UsernameAlreadyExistsException : Exception
    {
        public UsernameAlreadyExistsException()
        {
        }

        protected UsernameAlreadyExistsException(string message)
            : base(message)
        {
        }

        protected UsernameAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected UsernameAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}