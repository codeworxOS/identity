using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Test
{
    public class DummyLoginRegistrationProvider : ILoginRegistrationProvider
    {
        public Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
        {
            return Task.FromResult<IEnumerable<ILoginRegistration>>(
                new ILoginRegistration[] {
                    new DummyFormsLoginRegistration(),
                    new DummyWindowsLoginRegistration()
                });
        }
    }
}