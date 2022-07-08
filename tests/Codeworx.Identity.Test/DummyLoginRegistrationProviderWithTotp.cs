using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Login;
using Codeworx.Identity.Mfa.Totp;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Test
{
    public class DummyLoginRegistrationProviderWithTotp : DummyLoginRegistrationProvider
    {
        public override async Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(LoginProviderType loginProviderType, string userName = null)
        {
            switch (loginProviderType)
            {
                case LoginProviderType.Login:
                    return await base.GetLoginRegistrationsAsync(loginProviderType, userName);

                case LoginProviderType.MultiFactor:
                    {
                        var multiFactorLoginRegistrations = new List<ILoginRegistration>();
                        multiFactorLoginRegistrations.Add(new DummyTotpRegistration());
                        return multiFactorLoginRegistrations;
                    }

                default:
                    break;
            }

            throw new NotSupportedException($"Login provider type {loginProviderType} not supported!");
        }

        public class DummyTotpRegistration : ILoginRegistration
        {
            public Type ProcessorType => typeof(TotpMfaLoginProcessor);

            public string Name => TestConstants.LoginProviders.TotpProvider.Name;

            public string Id => TestConstants.LoginProviders.TotpProvider.Id;

            public object ProcessorConfiguration => null;
        }
    }
}

