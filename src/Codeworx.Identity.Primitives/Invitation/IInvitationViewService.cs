using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Invitation
{
    public interface IInvitationViewService
    {
        Task<InvitationViewResponse> ProcessAsync(InvitationViewRequest request);
    }
}
