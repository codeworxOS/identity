using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IViewTemplate
    {
        Task<string> GetLoggedInTemplate(string returnUrl);

        Task<string> GetLoginTemplate();

        Task<string> GetTenantSelectionTemplate();

        Task<string> GetFormPostTemplate(string redirectUrl, string code, string state);
    }
}