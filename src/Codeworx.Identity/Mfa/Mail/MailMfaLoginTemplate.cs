namespace Codeworx.Identity.Mfa.Mail
{
    public class MailMfaLoginTemplate : IPartialTemplate
    {
        public MailMfaLoginTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.account.mfa.mail_login.html");
        }

        public string Name => Constants.Templates.LoginMfaMail;

        public string Template { get; }
    }
}
