using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Configuration
{
    public class CertificateNotFoundException : Exception
    {
        public CertificateNotFoundException(string key)
            : base($"Certificate not found for search text: {key}")
        {
        }

        protected CertificateNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CertificateNotFoundException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }
    }
}
