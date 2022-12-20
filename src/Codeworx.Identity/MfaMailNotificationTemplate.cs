namespace Codeworx.Identity
{
    public class MfaMailNotificationTemplate : IPartialTemplate
    {
        public MfaMailNotificationTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.notification.mfa_mail.html");
        }

        public string Name => Constants.Templates.MfaMailNotification;

        public string Template { get; }
    }
}
