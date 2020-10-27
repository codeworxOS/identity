using System.Security.Claims;

namespace Codeworx.Identity.Login.Windows
{
    public class WindowsLoginRequest : ILoginRequest
    {
        public WindowsLoginRequest(ClaimsIdentity windowsIdentity, string returnUrl)
        {
            WindowsIdentity = windowsIdentity;
            ReturnUrl = returnUrl;
        }

        public string ReturnUrl { get; set; }

        public ClaimsIdentity WindowsIdentity { get; }

        public string ProviderId => Constants.ExternalWindowsProviderId;
    }
}