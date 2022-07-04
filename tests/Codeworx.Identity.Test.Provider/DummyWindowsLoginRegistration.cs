using System;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyWindowsLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType => typeof(WindowsLoginProcessor);

        public string Name => TestConstants.LoginProviders.ExternalWindowsProvider.Name;

        public string Id => TestConstants.LoginProviders.ExternalWindowsProvider.Id;

        public object ProcessorConfiguration => null;
    }
}