using System;

namespace Codeworx.Identity
{
    public abstract class ErrorResponseException : Exception
    {
        protected ErrorResponseException()
        {
        }

        public abstract Type ResponseType { get; }

        public abstract object Response { get; }
    }
}
