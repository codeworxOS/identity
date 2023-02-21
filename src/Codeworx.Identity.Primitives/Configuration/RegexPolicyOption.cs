using System.Collections.Generic;
using System.Linq;

namespace Codeworx.Identity.Configuration
{
    public class RegexPolicyOption
    {
        public RegexPolicyOption()
        {
            this.Description = new Dictionary<string, string>();
        }

        public RegexPolicyOption(RegexPolicyOption source)
        {
            this.Regex = source.Regex;
            this.Description = source.Description.ToDictionary(p => p.Key, p => p.Value);
        }

        public string Regex { get; set; }

        public Dictionary<string, string> Description { get; }
    }
}
