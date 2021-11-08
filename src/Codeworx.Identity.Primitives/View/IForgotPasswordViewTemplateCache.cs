using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IForgotPasswordViewTemplateCache
    {
        Task<string> GetForgotPasswordView(IDictionary<string, object> data);

        Task<string> GetForgotPasswordCompletedView(IDictionary<string, object> data);
    }
}
