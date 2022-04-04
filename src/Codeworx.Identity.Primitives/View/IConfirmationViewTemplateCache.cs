using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IConfirmationViewTemplateCache
    {
        Task<string> GetConfirmationView(IDictionary<string, object> data);
    }
}
