using System;
using Codeworx.Identity.Login;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyFormsLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType => typeof(FormsLoginProcessor);
        public string Name => TestConstants.LoginProviders.FormsLoginProvider.Name;

        public string Id => TestConstants.LoginProviders.FormsLoginProvider.Id;

        public object ProcessorConfiguration => null;
    }
}