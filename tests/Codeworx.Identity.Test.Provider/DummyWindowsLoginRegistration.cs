using System;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;

namespace Codeworx.Identity.Test
{
    public class DummyWindowsLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType => typeof(WindowsLoginProcessor);

        public string Name => Constants.ExternalWindowsProviderName;

        public string Id => Constants.ExternalWindowsProviderId;

        public object ProcessorConfiguration => null;
    }
}