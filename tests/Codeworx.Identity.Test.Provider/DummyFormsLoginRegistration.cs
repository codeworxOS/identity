using System;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Test
{
    public class DummyFormsLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType => typeof(FormsLoginProcessor);
        public string Name => Constants.FormsLoginProviderName;

        public string Id => Constants.FormsLoginProviderId;

        public object ProcessorConfiguration => null;
    }
}