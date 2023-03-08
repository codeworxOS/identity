namespace Codeworx.Identity.Model
{
    public class Terms
    {
        public Terms(bool showCheckbox, string text)
        {
            ShowCheckbox = showCheckbox;
            Text = text;
        }

        public bool ShowCheckbox { get; set; }

        public string Text { get; set; }
    }
}
