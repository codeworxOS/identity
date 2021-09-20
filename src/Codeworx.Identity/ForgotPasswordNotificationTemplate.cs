namespace Codeworx.Identity
{
    public class ForgotPasswordNotificationTemplate : IPartialTemplate
    {
        public ForgotPasswordNotificationTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.notification.forgot_password.html");
        }

        public string Name => Constants.Templates.ForgotPasswordNotification;

        public string Template { get; }
    }
}
