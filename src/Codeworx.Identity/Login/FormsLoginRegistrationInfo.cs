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
    }
}