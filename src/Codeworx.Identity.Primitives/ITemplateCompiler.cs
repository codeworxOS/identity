using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface ITemplateCompiler
    {
        Task<string> RenderAsync(string template, object data);
    }
}
