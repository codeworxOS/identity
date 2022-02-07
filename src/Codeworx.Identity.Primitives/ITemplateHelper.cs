using System.IO;

namespace Codeworx.Identity
{
    public interface ITemplateHelper
    {
        string Name { get; }

        void Process(TextWriter output, dynamic context, params object[] arguments);
    }
}
