using System;

namespace Codeworx.Identity.ExternalLogin
{
    public class WindowsLoginRegistration : IExternalLoginRegistration
    {
        public Type ProcessorType => typeof(WindowsLoginProcessor);

        public string Name => Constants.ExternalWindowsProviderName;

        public string Id => Constants.ExternalWindowsProviderId;

        public object ProcessorConfiguration => null;
    }
}
