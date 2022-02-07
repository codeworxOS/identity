using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cryptography.Argon2
{
    [Serializable]
    internal class HashingException : Exception
    {
        public HashingException(string message)
            : base(message)
        {
        }

        protected HashingException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected HashingException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}