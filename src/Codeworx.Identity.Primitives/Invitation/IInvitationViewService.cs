using System.Threading.Tasks;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Invitation
{
    public interface IInvitationViewService
    {
        Task<SignInResponse> ProcessAsync(ProcessInvitationViewRequest request);

        Task<InvitationViewResponse> ShowAsync(InvitationViewRequest request);
    }
}
