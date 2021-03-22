using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IInvitationViewTemplateCache
    {
        Task<string> GetInvitationView(IDictionary<string, object> data);
    }
}
