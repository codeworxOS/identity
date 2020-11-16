using System;
using Codeworx.Identity.Login.OAuth;

namespace Codeworx.Identity.Login
{
    public class ExternalOAuthLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => "oauth";

        public Type Type => typeof(OAuthLoginProcessor);

        public Type ConfigurationType => typeof(OAuthLoginConfiguration);
    }
}