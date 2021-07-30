using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cache
{
    public class InvalidCacheKeyFormatException : Exception
    {
        public InvalidCacheKeyFormatException()
            : this("Invalid cache key format!")
        {
        }

        protected InvalidCacheKeyFormatException(string message)
            : base(message)
        {
        }

        protected InvalidCacheKeyFormatException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected InvalidCacheKeyFormatException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
