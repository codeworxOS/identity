namespace Codeworx.Identity
{
    public class FormsProfileTemplate : IPartialTemplate
    {
        public FormsProfileTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.formsprofile.html");
        }

        public string Name => Constants.Templates.FormsProfile;

        public string Template { get; }
    }
}
