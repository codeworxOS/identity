using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

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

        public async Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IUser user, IIdentityDataParameters parameters)
        {
            var systemClaimsTasks = _systemClaimsProviders.Select(p => p.GetClaimsAsync(user, parameters));
            await Task.WhenAll(systemClaimsTasks).ConfigureAwait(false);

            var scopes = systemClaimsTasks.SelectMany(p => p.Result).ToList();

            if (_claimsProvider != null)
            {
                scopes.AddRange(await _claimsProvider.GetClaimsAsync(user, parameters).ConfigureAwait(false));
            }

            return scopes;
        }
    }
}
