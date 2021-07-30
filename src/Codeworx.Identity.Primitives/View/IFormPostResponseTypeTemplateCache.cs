using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IFormPostResponseTypeTemplateCache
    {
        Task<string> GetFormPostView(IDictionary<string, object> data);
    }
}
