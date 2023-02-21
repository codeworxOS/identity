using System;
using System.Runtime.Serialization;

namespace Codeworx.Identity.Configuration
{
    public class CertificateNotFoundException : Exception
    {
        public CertificateNotFoundException(SigningOptions options)
            : this($"Certificate not found for search text: {options?.Search}")
        {
            SigningOptions = options;
        }

        protected CertificateNotFoundException(string message)
            : base(message)
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

        public SigningOptions SigningOptions { get; }
    }
}
