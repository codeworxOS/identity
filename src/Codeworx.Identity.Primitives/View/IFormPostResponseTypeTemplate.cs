using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IFormPostResponseTypeTemplate
    {
        Task<string> GetFormPostTemplate();
    }
}
