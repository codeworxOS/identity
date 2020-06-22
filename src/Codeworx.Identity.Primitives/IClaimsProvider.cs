using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IClaimsProvider
    {
        Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters);
    }
}