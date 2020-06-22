using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity.OpenId
{
    public class OpenIdClaimsProvider : ISystemClaimsProvider
    {
        public OpenIdClaimsProvider()
        {
        }

        public Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            var result = new List<AssignedClaim>();

            if (parameters.Scopes.Contains(Constants.OpenId.Scopes.OpenId))
            {
                var upnClaim = AssignedClaim.Create(Constants.Claims.Upn, parameters.User.Name);
                result.Add(upnClaim);
            }

            return Task.FromResult<IEnumerable<AssignedClaim>>(result);
        }
    }
}
