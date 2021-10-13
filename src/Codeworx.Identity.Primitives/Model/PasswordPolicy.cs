using System;
using System.Collections.Generic;
using Codeworx.Identity.Resources;

namespace Codeworx.Identity.Model
{
    public class PasswordPolicy
    {
        private readonly IDictionary<string, string> _descriptions;

        public PasswordPolicy(string regex, IDictionary<string, string> descriptions)
        {
            this.Regex = regex;
            _descriptions = descriptions;
        }

        public string Regex { get; }

        public string GetDescription(IStringResources stringResources)
        {
            var languageCode = stringResources.GetResource(StringResource.LanguageCode);

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
                throw new ArgumentException($"No password policy description for language {languageCode}");
            }
        }
    }
}
