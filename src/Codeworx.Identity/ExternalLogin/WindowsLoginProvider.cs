using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Configuration;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.ExternalLogin
{
    public class WindowsLoginProvider : IExternalLoginProvider
    {
        private readonly bool _isEnabled;

        public WindowsLoginProvider(IOptionsSnapshot<IdentityOptions> options)
        {
            _isEnabled = options.Value.WindowsAuthenticationEnabled;
        }

        public Task<IEnumerable<IExternalLoginRegistration>> GetLoginRegistrationsAsync(string userName = null)
        {
            if (_isEnabled)
            {
                IEnumerable<IExternalLoginRegistration> windowsRegistration = new IExternalLoginRegistration[]
                {
                    new WindowsLoginRegistration()
                };

                return Task.FromResult(windowsRegistration);
            }

            return Task.FromResult(Enumerable.Empty<IExternalLoginRegistration>());
        }
    }
}
