using System;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaLoginProcessorLookup : IProcessorTypeLookup
    {
        public string Key => Constants.Processors.Mail;

        public Type Type { get; } = typeof(MailMfaLoginProcessor);

        public Type ConfigurationType => null;
    }
}
