using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface ILoginViewTemplate
    {
        Task<string> GetLoggedInTemplate();

        Task<string> GetLoginTemplate();
    }
}
