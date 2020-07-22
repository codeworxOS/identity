namespace Codeworx.Identity.Login
{
    public class RedirectRegistrationInfo : ILoginRegistrationInfo
    {
        public RedirectRegistrationInfo(string providerId, string name, string redirectUri)
        {
            Name = name;
            RedirectUri = redirectUri;
            ProviderId = providerId;
        }

        public string Name { get; }

        public string RedirectUri { get; }

        public string ProviderId { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = RedirectUri;
            return true;
        }
    }
}
