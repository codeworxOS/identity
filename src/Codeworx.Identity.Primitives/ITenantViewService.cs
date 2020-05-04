using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity
{
    public interface ITenantViewService
    {
        Task<SelectTenantViewResponse> ShowAsync(SelectTenantViewRequest request);

        Task<SelectTenantSuccessResponse> SelectAsync(SelectTenantViewActionRequest request);
    }
}
