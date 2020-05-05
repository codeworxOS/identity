using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface IClaimsProvider
    {
        Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IUser user, IIdentityDataParameters parameters);
    }
}