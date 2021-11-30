namespace Codeworx.Identity
{
    public class NewInvitationNotificationTemplate : IPartialTemplate
    {
        public NewInvitationNotificationTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.notification.new_invitation.html");
        }

        public string Name => Constants.Templates.NewInvitationNotification;

        public string Template { get; }
    }
}
