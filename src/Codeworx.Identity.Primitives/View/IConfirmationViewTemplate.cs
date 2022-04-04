using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IConfirmationViewTemplate
    {
        Task<string> GetConfirmationViewTemplate();
    }
}
