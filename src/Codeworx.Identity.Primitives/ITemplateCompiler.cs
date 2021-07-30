using System;

namespace Codeworx.Identity
{
    public interface ITemplateCompiler
    {
        Func<object, string> Compile(string template);
    }
}
