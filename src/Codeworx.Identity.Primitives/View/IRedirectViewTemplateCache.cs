using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IRedirectViewTemplateCache
    {
        Task<string> GetRedirectView(IDictionary<string, object> data);
    }
}
