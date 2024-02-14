using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public class ScimClaimsProvider : ISystemClaimsProvider
    {
        public ScimClaimsProvider()
        {
        }

        public Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            if (parameters.Client.ClientType.HasFlag(Model.ClientType.ApiKey) && parameters.Client.AllowScim)
            {
                return Task.FromResult<IEnumerable<AssignedClaim>>(
                    new[] { AssignedClaim.Create(Constants.Claims.Scim, Constants.Claims.Values.Allow, ClaimTarget.AccessToken | ClaimTarget.ProfileEndpoint) });
            }

            return Task.FromResult(Enumerable.Empty<AssignedClaim>());
        }
    }
}
