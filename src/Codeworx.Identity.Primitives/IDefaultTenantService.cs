using System.Threading.Tasks;

namespace Codeworx.Identity
{
    public interface IDefaultTenantService
    {
        Task SetDefaultTenantAsync(string identifier, string tenantKey);
    }
}