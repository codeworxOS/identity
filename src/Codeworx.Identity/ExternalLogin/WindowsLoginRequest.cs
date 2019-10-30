using System.Security.Claims;

namespace Codeworx.Identity.ExternalLogin
{
    public class WindowsLoginRequest
    {
        public WindowsLoginRequest(ClaimsIdentity windowsIdentity, string returnUrl)
        {
            WindowsIdentity = windowsIdentity;
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; set; }

        public ClaimsIdentity WindowsIdentity { get; }
    }
}