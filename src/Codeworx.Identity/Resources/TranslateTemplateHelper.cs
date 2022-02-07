using System;
using System.IO;

namespace Codeworx.Identity.Resources
{
    public class TranslateTemplateHelper : ITemplateHelper
    {
        private readonly IStringResources _stringResources;

        public TranslateTemplateHelper(IStringResources stringResources)
        {
            _stringResources = stringResources;
        }

        public string Name => "Translate";

        public void Process(TextWriter output, dynamic context, params object[] arguments)
        {
            if (arguments.Length == 1 && arguments[0] is string resource)
            {
                if (Enum.TryParse<StringResource>(resource, out var stringResource))
                {
                    output.Write(_stringResources.GetResource(stringResource));
                }
            }
        }
    }
}
