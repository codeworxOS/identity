using System.Threading.Tasks;

namespace Codeworx.Identity.View
{
    public interface IInvitationViewTemplate
    {
        Task<string> GetInvitationTemplate();
    }
}
