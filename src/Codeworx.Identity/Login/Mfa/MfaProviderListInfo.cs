namespace Codeworx.Identity.Login.Mfa
{
    public class MfaProviderListInfo : ILoginRegistrationInfo
    {
        public MfaProviderListInfo(string providerId, string providerUrl, string description, string cssClass, string error)
        {
            ProviderId = providerId;
            ProviderUrl = providerUrl;
            Description = description;
            CssClass = cssClass;
            Error = error;
        }

        public string Template => Constants.Templates.MfaList;

        public string Error { get; }

        public string ProviderId { get; }

        public string ProviderUrl { get; }

        public string Description { get; }

        public string CssClass { get; }

        public bool HasRedirectUri(out string redirectUri)
        {
            redirectUri = ProviderUrl;
            return true;
        }
    }
}
