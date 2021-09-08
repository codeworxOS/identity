using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IProfileViewTemplateCache
    {
        Task<string> GetProfileView(IDictionary<string, object> data);
    }
}
