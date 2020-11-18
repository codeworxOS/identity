namespace Codeworx.Identity.Login
{
    internal class FormsLoginRegistrationInfo : ILoginRegistrationInfo
    {
        public FormsLoginRegistrationInfo(string providerId, string userName, string error = null)
        {
            UserName = userName;
            ProviderId = providerId;
            Error = error;
        }

        public string UserName { get; }

        public string ProviderId { get; }

        public string Error { get; }

        public bool HasRedirectUri(out string redirectUrl)
        {
            redirectUrl = null;
            return false;
        }
    }
}