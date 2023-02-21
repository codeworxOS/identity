using System.Collections.Generic;
using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface ILoginViewTemplateCache
    {
        Task<string> GetLoginView(IDictionary<string, object> data);

        Task<string> GetChallengeResponse(IDictionary<string, object> data);

        Task<string> GetMfaOverview(IDictionary<string, object> data);

        Task<string> GetMfaProvider(IDictionary<string, object> data);
    }
}
