using System.Security.Principal;

namespace Codeworx.Identity.AspNetCore
{
    public class AuthenticatedUserInformation
    {
        public IPrincipal Principal { get; set; }
    }
}
