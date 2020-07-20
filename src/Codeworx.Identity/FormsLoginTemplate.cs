namespace Codeworx.Identity
{
    public class FormsLoginTemplate : IPartialTemplate
    {
        public FormsLoginTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.formslogin.html");
        }

        public string Name => Constants.Templates.FormsLogin;

        public string Template { get; }
    }
}
