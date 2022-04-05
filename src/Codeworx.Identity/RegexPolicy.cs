using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public class RegexPolicy : IStringPolicy
    {
        private readonly IDictionary<string, string> _descriptions;

        public RegexPolicy(string regex, IDictionary<string, string> descriptions)
        {
            this.Regex = new Regex(regex);
            _descriptions = descriptions;
        }

        public RegexPolicy(Regex regex, IDictionary<string, string> descriptions)
        {
            this.Regex = regex;
            _descriptions = descriptions;
        }

        public virtual Regex Regex { get; }

        public virtual string GetDescription(string languageCode)
        {
            if (_descriptions.TryGetValue(languageCode, out var description))
            {
                return description;
            }
            else if (_descriptions.TryGetValue("en", out var defaultDescription))
            {
                return defaultDescription;
            }
            else
            {
                throw new ArgumentException($"No policy description for language {languageCode} found.");
            }
        }

        public bool IsValid(string value, string languageCode, out string validationMessage)
        {
            if (this.Regex.IsMatch(value))
            {
                validationMessage = null;
                return true;
            }

            validationMessage = GetDescription(languageCode);
            return false;
        }
    }
}
