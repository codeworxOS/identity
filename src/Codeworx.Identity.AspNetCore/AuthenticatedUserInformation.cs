using System.Security.Claims;
using System.Security.Principal;

namespace Codeworx.Identity.AspNetCore
{
    public class AuthenticatedUserInformation
    {
        private IPrincipal _principal;

        public IPrincipal Principal
        {
            get => _principal;
            set
            {
                _principal = value;

                if (_principal?.Identity is ClaimsIdentity claimsIdentity)
                {
                    this.IdentityData = claimsIdentity.ToIdentityData();
                }
                else
                {
                    this.IdentityData = null;
                }
            }
        }

        public IdentityData IdentityData { get; private set; }
    }
}
