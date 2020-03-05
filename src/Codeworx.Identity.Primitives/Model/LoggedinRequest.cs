using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class LoggedinRequest : LoginRequest
    {
        public LoggedinRequest(ClaimsIdentity identity, string returnUrl)
            : base(returnUrl)
        {
            Identity = identity;
        }

        public ClaimsIdentity Identity { get; }
    }
}