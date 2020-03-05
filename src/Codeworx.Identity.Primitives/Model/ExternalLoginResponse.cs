using System.Collections.Generic;
using System.Linq;

namespace Codeworx.Identity.Model
{
    public class ExternalLoginResponse
    {
        public ExternalLoginResponse(string nameIdentifier, string returnUrl, IEnumerable<AssignedClaim> claims = null)
        {
            NameIdentifier = nameIdentifier;
            ReturnUrl = returnUrl;
            Claims = claims ?? Enumerable.Empty<AssignedClaim>();
        }

        public IEnumerable<AssignedClaim> Claims { get; }

        public string NameIdentifier { get; }

        public string ReturnUrl { get; set; }
    }
}