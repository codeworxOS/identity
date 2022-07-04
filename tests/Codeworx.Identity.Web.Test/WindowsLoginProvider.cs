using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.ExternalLogin;
using Codeworx.Identity.Login;
using Codeworx.Identity.Login.Windows;

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
                        Id = Constants.TestData.LoginProviders.ExternalWindowsProvider.Id,
                        Name = Constants.TestData.LoginProviders.ExternalWindowsProvider.Name,
                        ProcessorType = typeof(WindowsLoginProcessor)
                    }
                });
        }
    }
}