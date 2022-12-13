namespace Codeworx.Identity.Mfa
{
    public class MfaProviderListTemplate : IPartialTemplate
    {
        public MfaProviderListTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.account.mfa_list.html");
        }

        public string Name => Constants.Templates.MfaList;

        public string Template { get; }
    }
}
