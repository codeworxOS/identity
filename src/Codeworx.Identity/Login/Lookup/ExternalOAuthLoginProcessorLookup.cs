using System;
using Codeworx.Identity.Login.OAuth;

namespace Codeworx.Identity.Login
{
    public class ExternalOAuthLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => Constants.Processors.OAuth;

        public Type Type => typeof(OAuthLoginProcessor);

        public Type ConfigurationType => typeof(OAuthLoginConfiguration);
    }
}