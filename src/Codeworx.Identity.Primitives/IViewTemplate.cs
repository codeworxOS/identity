using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IViewTemplate
    {
        Task<string> GetLoggedInTemplate(string returnUrl);

        Task<string> GetLoginTemplate(string returnUrl, string username = null, string error = null);

        Task<string> GetTenantSelectionTemplate(string returnUrl, bool showDefault);

        Task<string> GetFormPostTemplate(string redirectUrl, string code, string state);
    }
}