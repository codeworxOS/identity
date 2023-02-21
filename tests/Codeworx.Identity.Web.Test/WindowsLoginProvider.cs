using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.ExternalLogin;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;
using Codeworx.Identity.Test.Provider;

namespace Codeworx.Identity.Web.Test
{
    public class WindowsLoginProvider : ILoginRegistrationProvider
    {
        public Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(LoginProviderType loginProviderType, string userName = null)
        {
            return Task.FromResult<IEnumerable<ILoginRegistration>>(
                new[] {
                    new ExternalLoginRegistration
                    {
                        Id = TestConstants.LoginProviders.ExternalWindowsProvider.Id,
                        Name = TestConstants.LoginProviders.ExternalWindowsProvider.Name,
                        ProcessorType = typeof(WindowsLoginProcessor)
                    }
                });
        }
    }
}