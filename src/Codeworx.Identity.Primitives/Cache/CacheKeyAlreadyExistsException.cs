using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cache
{
    public class CacheKeyAlreadyExistsException : Exception
    {
        public CacheKeyAlreadyExistsException()
            : this("The provided Key already exists.")
        {
        }

        public CacheKeyAlreadyExistsException(string message)
            : base(message)
        {
        }

        public CacheKeyAlreadyExistsException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CacheKeyAlreadyExistsException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
