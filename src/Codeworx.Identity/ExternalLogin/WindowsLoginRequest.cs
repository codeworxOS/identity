using System.Security.Claims;

namespace Codeworx.Identity.ExternalLogin
{
    public class WindowsLoginRequest
    {
        public WindowsLoginRequest(ClaimsIdentity windowsIdentity)
        {
            WindowsIdentity = windowsIdentity;
        }

        public ClaimsIdentity WindowsIdentity { get; }
    }
}
