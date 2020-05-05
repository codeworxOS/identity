using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth
{
    public class SystemClaimsProvider : ISystemClaimsProvider
    {
        private readonly IBaseUriAccessor _baseUri;

        public SystemClaimsProvider(IBaseUriAccessor baseUri)
        {
            _baseUri = baseUri;
        }

        public Task<IEnumerable<AssignedClaim>> GetClaimsAsync(IUser user, IIdentityDataParameters parameters)
        {
            var result = new List<AssignedClaim>();

            var subjectClaim = AssignedClaim.Create(Constants.Claims.Subject, user.Identity);
            result.Add(subjectClaim);

            var issuerClaim = AssignedClaim.Create(Constants.Claims.Issuer, _baseUri.BaseUri.ToString());
            result.Add(issuerClaim);

            var audienceClaim = AssignedClaim.Create(Constants.Claims.Audience, parameters.ClientId);
            result.Add(audienceClaim);

            if (parameters.Scopes.Any())
            {
                var scopeClaim = AssignedClaim.Create(Constants.OAuth.ScopeName, parameters.Scopes);
                result.Add(scopeClaim);
            }

            if (!string.IsNullOrWhiteSpace(parameters.Nonce))
            {
                var nonceClaim = AssignedClaim.Create(Constants.OAuth.NonceName, parameters.Nonce);
                result.Add(nonceClaim);
            }

            if (!string.IsNullOrWhiteSpace(parameters.State))
            {
                var stateClaim = AssignedClaim.Create(Constants.OAuth.StateName, parameters.State);
                result.Add(stateClaim);
            }

            return Task.FromResult<IEnumerable<AssignedClaim>>(result);
        }
    }
}
