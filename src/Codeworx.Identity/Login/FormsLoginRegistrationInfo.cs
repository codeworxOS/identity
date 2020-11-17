namespace Codeworx.Identity.Login
{
    internal class FormsLoginRegistrationInfo : ILoginRegistrationInfo
    {
        public FormsLoginRegistrationInfo(string userName)
        {
            UserName = userName;
        }

        public string UserName { get; }

        public string ProviderId => Constants.FormsLoginProviderId;

        public string Error => null;

        public bool HasRedirectUri(out string redirectUrl)
        {
            redirectUrl = null;
            return false;
        }
    }
}