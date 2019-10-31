using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.ExternalLogin
{
    public interface IExternalLoginProvider
    {
        Task<IEnumerable<IExternalLoginRegistration>> GetLoginRegistrationsAsync(string userName = null);
    }
}