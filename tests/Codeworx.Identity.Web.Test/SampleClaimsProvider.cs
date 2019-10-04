using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Web.Test
{
    public class SampleClaimsProvider : IClaimsService
    {
        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IUser user, string tenantKey = null)
        {
            await Task.Yield();

            return new[]
            {
                new AssignedClaim("whatever",new []{"Raphael Schwarz" },ClaimTarget.AllTokens, AssignedClaim.AssignmentSource.User)
            };
        }
    }
}