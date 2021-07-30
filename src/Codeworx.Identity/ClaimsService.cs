using System.Collections.Generic;
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
            var claims = new List<AssignedClaim>();
            foreach (var provider in _systemClaimsProviders)
            {
                claims.AddRange(await provider.GetClaimsAsync(parameters).ConfigureAwait(false));
            }

            if (_claimsProvider != null)
            {
                claims.AddRange(await _claimsProvider.GetClaimsAsync(parameters).ConfigureAwait(false));
            }

            return claims;
        }
    }
}
