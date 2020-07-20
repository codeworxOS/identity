namespace Codeworx.Identity
{
    public class RedirectLinkTemplate : IPartialTemplate
    {
        public RedirectLinkTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.redirect_link.html");
        }

        public string Name => Constants.Templates.Redirect;

        public string Template { get; }
    }
}
