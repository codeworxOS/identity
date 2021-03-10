namespace Codeworx.Identity
{
    public class FormsInvitationTemplate : IPartialTemplate
    {
        public FormsInvitationTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.formsinvitation.html");
        }

        public string Name => Constants.Templates.FormsInvitation;

        public string Template { get; }
    }
}
