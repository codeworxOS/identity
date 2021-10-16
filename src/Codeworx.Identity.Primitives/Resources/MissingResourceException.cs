using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Resources
{
    public class MissingResourceException : Exception
    {
        public MissingResourceException(StringResource resource)
            : this($"Resource {resource} is missing!")
        {
            Resource = resource;
        }

        protected MissingResourceException(string message)
            : base(message)
        {
        }

        protected MissingResourceException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MissingResourceException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        public StringResource Resource { get; }
    }
}
