using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface ITenantViewTemplateCache
    {
        Task<string> GetTenantSelection(IDictionary<string, object> data);
    }
}
