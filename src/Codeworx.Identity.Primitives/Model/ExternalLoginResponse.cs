using System.Collections.Generic;
using System.Linq;

namespace Codeworx.Identity.Model
{
    public class ExternalLoginResponse
    {
        public ExternalLoginResponse(string nameIdentifier, IEnumerable<AssignedClaim> claims = null)
        {
            NameIdentifier = nameIdentifier;
            Claims = claims ?? Enumerable.Empty<AssignedClaim>();
        }

        public string NameIdentifier { get; }

        public IEnumerable<AssignedClaim> Claims { get; }
    }
}
