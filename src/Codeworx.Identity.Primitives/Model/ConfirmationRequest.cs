using System.Security.Claims;

namespace Codeworx.Identity.Model
{
    public class ConfirmationRequest
    {
        public ConfirmationRequest(ClaimsIdentity identity, string confirmationCode)
        {
            Identity = identity;
            ConfirmationCode = confirmationCode;
        }

        public ClaimsIdentity Identity { get; }

        public string ConfirmationCode { get; }
    }
}
