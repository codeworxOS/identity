using System;
using Codeworx.Identity;
using Codeworx.Identity.Login;
using Codeworx.Identity.Mfa.Totp;

namespace Microsoft.Extensions.DependencyInjection
{
    public class TotpMfaLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => Constants.Processors.Totp;

        public Type Type => typeof(TotpMfaLoginProcessor);

        public Type ConfigurationType => null;
    }
}