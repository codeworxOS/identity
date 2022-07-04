using System;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;

namespace Codeworx.Identity.Test
{
    public class DummyWindowsLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType => typeof(WindowsLoginProcessor);

        public string Name => Constants.TestData.LoginProviders.ExternalWindowsProvider.Name;

        public string Id => Constants.TestData.LoginProviders.ExternalWindowsProvider.Id;

        public object ProcessorConfiguration => null;
    }
}