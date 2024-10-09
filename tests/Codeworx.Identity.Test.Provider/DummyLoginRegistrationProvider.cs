using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.Test
{
    public class DummyLoginRegistrationProvider : ILoginRegistrationProvider
    {
        public virtual Task<IEnumerable<ILoginRegistration>> GetLoginRegistrationsAsync(LoginProviderType loginProviderType, string userName = null)
        {
            return Task.FromResult<IEnumerable<ILoginRegistration>>(
                new ILoginRegistration[] {
                    new DummyFormsLoginRegistration(),
                    new DummyWindowsLoginRegistration(),
                    new DummyExternalOAuthLoginRegistration()
                });
        }
    }
}