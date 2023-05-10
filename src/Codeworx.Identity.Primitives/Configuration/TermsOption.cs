using System.Collections.Generic;

namespace Codeworx.Identity.Configuration
{
    public class TermsOption
    {
        public TermsOption()
        {
            this.Text = new Dictionary<string, string>();
        }

        public TermsOption(TermsOption other)
            : this()
        {
            Mode = other.Mode;
            foreach (var text in other.Text)
            {
                Text.Add(text.Key, text.Value);
            }
        }

        public enum TermsMode
        {
            None = 0,
            Show = 1,
            Confirm = 2,
        }

        public TermsMode Mode { get; set; }

        public Dictionary<string, string> Text { get; set; }
    }
}