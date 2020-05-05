using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Stubble.Core.Interfaces;

namespace Codeworx.Identity.AspNetCore
{
    public class StyleTemplateLoader : IStubbleLoader
    {
        private readonly ImmutableList<string> _styles;

        public StyleTemplateLoader(IEnumerable<string> styles)
        {
            _styles = styles.ToImmutableList();
        }

        public IStubbleLoader Clone()
        {
            return new StyleTemplateLoader(_styles);
        }

        public string Load(string name)
        {
            if (name == "Styles")
            {
                return string.Join("\r\n", _styles.Select(p => $"<link type=\"text/css\" rel=\"stylesheet\" href=\"{p}\" >"));
            }

            return null;
        }

        public ValueTask<string> LoadAsync(string name)
        {
            return new ValueTask<string>(Load(name));
        }
    }
}
