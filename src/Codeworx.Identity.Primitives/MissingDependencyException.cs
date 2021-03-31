using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity
{
    public class MissingDependencyException : Exception
    {
        public MissingDependencyException(Type dependency)
            : this($"Missing Dependency {dependency}")
        {
        }

        protected MissingDependencyException(string message)
            : base(message)
        {
        }

        protected MissingDependencyException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected MissingDependencyException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
