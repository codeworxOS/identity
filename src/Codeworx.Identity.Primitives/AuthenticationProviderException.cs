using System;

namespace Codeworx.Identity
{
    public class AuthenticationProviderException : Exception
    {
        public AuthenticationProviderException(string provider)
        : base($"Missing provider with id {provider}")
        {
            this.Provider = provider;
        }

        public string Provider { get; }
    }
}