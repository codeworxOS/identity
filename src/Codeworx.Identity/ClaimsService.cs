using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public class ClaimsService : IClaimsService
    {
        private readonly IEnumerable<ISystemClaimsProvider> _systemClaimsProviders;
        private readonly IClaimsProvider _claimsProvider;

        public ClaimsService(IEnumerable<ISystemClaimsProvider> systemClaimsProviders, IClaimsProvider claimsProvider = null)
        {
            _systemClaimsProviders = systemClaimsProviders;
            _claimsProvider = claimsProvider;
        }

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IIdentityDataParameters parameters)
        {
            var systemClaimsTasks = _systemClaimsProviders.Select(p => p.GetClaimsAsync(parameters)).ToList();
            await Task.WhenAll(systemClaimsTasks).ConfigureAwait(false);

            var claims = systemClaimsTasks.SelectMany(p => p.Result).ToList();

            if (_claimsProvider != null)
            {
                claims.AddRange(await _claimsProvider.GetClaimsAsync(parameters).ConfigureAwait(false));
            }

            return claims;
        }
    }
}
