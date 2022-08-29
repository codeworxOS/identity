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
                var upnSourceClaim = parameters.User.FindFirst(Constants.Claims.Upn);
                if (upnSourceClaim != null)
                {
                    var upnClaim = AssignedClaim.Create(Constants.Claims.Upn, upnSourceClaim.Value);
                    result.Add(upnClaim);
                }
            }

            var amrClaims = parameters.User.FindAll(Constants.Claims.Amr).ToList();

            if (amrClaims.Any())
            {
                var amrClaim = AssignedClaim.Create(Constants.Claims.Amr, amrClaims.Select(p => p.Value));
                result.Add(amrClaim);
            }

            return Task.FromResult<IEnumerable<AssignedClaim>>(result);
        }
    }
}
