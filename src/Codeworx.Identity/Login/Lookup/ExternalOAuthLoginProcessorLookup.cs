using System;

namespace Codeworx.Identity.Login
{
    public class ExternalOAuthLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => "oauth";

        public Type Type => typeof(ExternalOAuthLoginProcessor);
    }
}