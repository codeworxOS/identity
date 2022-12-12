namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaRegistrationTemplate : IPartialTemplate
    {
        public MailMfaRegistrationTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.account.mfa.mail_registration.html");
        }

        public string Name => Constants.Templates.RegisterMfaMail;

        public string Template { get; }
    }
}
