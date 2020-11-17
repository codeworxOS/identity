using System.Security.Claims;

namespace Codeworx.Identity.Login.Windows
{
    public class WindowsLoginRequest
    {
        public WindowsLoginRequest(string providerId, ClaimsIdentity windowsIdentity, string returnUrl)
        {
            WindowsIdentity = windowsIdentity;
            ReturnUrl = returnUrl;
            ProviderId = providerId;
        }

        public string ReturnUrl { get; set; }

        public ClaimsIdentity WindowsIdentity { get; }

        public string ProviderId { get; }
    }
}