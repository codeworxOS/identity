using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IRedirectViewTemplate
    {
        Task<string> GetRedirectTemplate();
    }
}
