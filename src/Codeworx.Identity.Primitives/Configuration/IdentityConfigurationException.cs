using System;

namespace Codeworx.Identity.Configuration
{
    public class IdentityConfigurationException : Exception
    {
        public IdentityConfigurationException()
            : this(Constants.ConfigrationExceptionMessage)
        {
        }

        public IdentityConfigurationException(string message)
            : base(message)
        {
        }

        public IdentityConfigurationException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}