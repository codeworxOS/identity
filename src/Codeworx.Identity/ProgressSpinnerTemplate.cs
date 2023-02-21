namespace Codeworx.Identity
{
    public class ProgressSpinnerTemplate : IPartialTemplate
    {
        public ProgressSpinnerTemplate()
        {
            Template = DefaultViewTemplate.GetTemplateAsString("Codeworx.Identity.assets.progress_spinner.html");
        }

        public string Name => Constants.Templates.ProgressSpinner;

        public string Template { get; }
    }
}
