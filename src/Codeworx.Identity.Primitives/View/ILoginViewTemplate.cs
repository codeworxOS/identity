using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface ILoginViewTemplate
    {
        Task<string> GetLoginTemplate();

        Task<string> GetMfaOverviewTemplate();

        Task<string> GetMfaProviderTemplate();

        Task<string> GetChallengeResponse();
    }
}
