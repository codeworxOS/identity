using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.ExternalLogin;
using Codeworx.Identity.ExternalLogin;

namespace Codeworx.Identity.Web.Test
{
    public class WindowsLoginProvider : IExternalLoginProvider
    {
        public Task<IEnumerable<IExternalLoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
        {
            return Task.FromResult<IEnumerable<IExternalLoginRegistration>>(
                new[] {
                    new ExternalLoginRegistration
                    {
                        Id = Constants.ExternalWindowsProviderId,
                        Name = Constants.ExternalWindowsProviderName,
                        ProcessorType = typeof(WindowsLoginProcessor)
                    }
                });
        }
    }
}