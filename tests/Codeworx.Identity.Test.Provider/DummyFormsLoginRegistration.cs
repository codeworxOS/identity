using System;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Test
{
    public class DummyFormsLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType => typeof(FormsLoginProcessor);
        public string Name => Constants.TestData.LoginProviders.FormsLoginProvider.Name;

        public string Id => Constants.TestData.LoginProviders.FormsLoginProvider.Id;

        public object ProcessorConfiguration => null;
    }
}