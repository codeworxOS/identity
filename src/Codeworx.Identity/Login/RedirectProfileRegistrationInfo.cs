namespace Codeworx.Identity.Login
{
    public class RedirectProfileRegistrationInfo : RedirectRegistrationInfo
    {
        public RedirectProfileRegistrationInfo(string providerId, string name, string cssClass, string redirectUri, bool isLinked, string error = null)
            : base(providerId, name, cssClass, redirectUri, error)
        {
            IsLinked = isLinked;
        }

        public override string Template => Constants.Templates.RedirectProfile;

        public bool IsLinked { get; }
    }
}
