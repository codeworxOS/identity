namespace Codeworx.Identity.Login
{
    public class RedirectProfileRegistrationInfo : RedirectRegistrationInfo
    {
        public RedirectProfileRegistrationInfo(string providerId, string name, string redirectUri, string error = null)
            : base(providerId, name, redirectUri, error)
        {
        }
    }
}
