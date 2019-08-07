using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IViewTemplate
    {
        Task<string> GetLoggedInTemplate(string returnUrl);

        Task<string> GetLoginTemplate(string returnUrl);
    }
}