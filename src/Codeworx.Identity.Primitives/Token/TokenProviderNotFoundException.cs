using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Token
{
    public class TokenProviderNotFoundException : Exception
    {
        public TokenProviderNotFoundException(string tokenFormat)
            : base($"Token provider for token format {tokenFormat} not found.")
        {
        }

        protected TokenProviderNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected TokenProviderNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
