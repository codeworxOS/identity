using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IViewTemplate
    {
        Task<string> GetConsentTemplate();

        Task<string> GetLoggedinTemplate(string returnUrl);

        Task<string> GetLoginTemplate(string returnUrl);
    }
}