using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class ConfirmationRequest
    {
        public ConfirmationRequest(ClaimsIdentity identity, string confirmationCode, bool rememberMe)
        {
            Identity = identity;
            ConfirmationCode = confirmationCode;
            RememberMe = rememberMe;
        }

        public ClaimsIdentity Identity { get; }

        public string ConfirmationCode { get; }

        public bool RememberMe { get; }
    }
}
