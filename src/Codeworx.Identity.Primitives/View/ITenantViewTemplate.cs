using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface ITenantViewTemplate
    {
        Task<string> GetTenantSelectionTemplate();
    }
}
