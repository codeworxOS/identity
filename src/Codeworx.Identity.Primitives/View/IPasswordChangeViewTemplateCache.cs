using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IPasswordChangeViewTemplateCache
    {
        Task<string> GetPasswordChangeView(IDictionary<string, object> data);
    }
}
