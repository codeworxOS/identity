using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Cache
{
    public class CacheEntryNotFoundException : Exception
    {
        public CacheEntryNotFoundException()
            : this("The given key was not found in the cache.")
        {
        }

        public CacheEntryNotFoundException(string message)
            : base(message)
        {
        }

        public CacheEntryNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CacheEntryNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
