namespace Codeworx.Identity.Login
{
    public class RedirectRegistrationInfo : ILoginRegistrationInfo
    {
        public RedirectRegistrationInfo(string providerId, string name, string redirectUri, string error = null)
        {
            Name = name;
            RedirectUri = redirectUri;
            ProviderId = providerId;
            Error = error;
            Template = Constants.Templates.Redirect;
        }

        public string Template { get; }

        public string Name { get; }

        public string RedirectUri { get; }

        public string ProviderId { get; }

        public string Error { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = RedirectUri;
            return true;
        }
    }
}
