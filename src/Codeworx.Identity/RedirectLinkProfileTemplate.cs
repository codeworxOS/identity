namespace Codeworx.Identity
{
    public class RedirectLinkProfileTemplate : IPartialTemplate
    {
        public RedirectLinkProfileTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.redirect_link_profile.html");
        }

        public string Name => Constants.Templates.RedirectProfile;

        public string Template { get; }
    }
}
