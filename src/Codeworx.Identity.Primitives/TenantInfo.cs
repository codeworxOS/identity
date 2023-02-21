using Codeworx.Identity.Login;

namespace Codeworx.Identity
{
    public class TenantInfo
    {
        public string Key { get; set; }

        public string Name { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }
    }
}