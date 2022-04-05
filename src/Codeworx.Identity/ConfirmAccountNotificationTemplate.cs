namespace Codeworx.Identity
{
    public class ConfirmAccountNotificationTemplate : IPartialTemplate
    {
        public ConfirmAccountNotificationTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.notification.confirm_account.html");
        }

        public string Name => Constants.Templates.ConfirmAccountNotification;

        public string Template { get; }
    }
}
