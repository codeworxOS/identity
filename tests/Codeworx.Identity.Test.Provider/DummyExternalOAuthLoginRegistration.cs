using System;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.OAuth;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyExternalOAuthLoginRegistration : ILoginRegistration
    {
        public Type ProcessorType => typeof(OAuthLoginProcessor);

        public string Name => TestConstants.LoginProviders.ExternalOAuthProvider.Name;

        public string Id => TestConstants.LoginProviders.ExternalOAuthProvider.Id;

        public object ProcessorConfiguration => new OAuthLoginConfiguration
        {
            BaseUri = new Uri("https://login.external/test")
        };
    }
}