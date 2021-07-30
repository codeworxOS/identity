using System.Security.Claims;

namespace Codeworx.Identity.Login.Windows
{
    public class WindowsLoginRequest
    {
        public WindowsLoginRequest(string providerId, ClaimsIdentity windowsIdentity, string returnUrl, string invitationCode)
        {
            WindowsIdentity = windowsIdentity;
            ReturnUrl = returnUrl;
            ProviderId = providerId;
            InvitationCode = invitationCode;
        }

        public string ReturnUrl { get; set; }

        public ClaimsIdentity WindowsIdentity { get; }

        public string ProviderId { get; }

        public string InvitationCode { get; }
    }
}