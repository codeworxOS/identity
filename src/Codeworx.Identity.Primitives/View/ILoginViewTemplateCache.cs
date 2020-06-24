using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface ILoginViewTemplateCache
    {
        Task<string> GetLoggedInView(IDictionary<string, object> data);

        Task<string> GetLoginView(IDictionary<string, object> data);
    }
}
