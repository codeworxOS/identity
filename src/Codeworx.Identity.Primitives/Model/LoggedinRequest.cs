using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class LoggedinRequest : LoginRequest
    {
        public LoggedinRequest(ClaimsIdentity identity, string returnUrl, string prompt)
            : base(returnUrl, prompt)
        {
            Identity = identity;
        }

        public ClaimsIdentity Identity { get; }
    }
}