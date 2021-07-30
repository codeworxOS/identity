using System.Threading.Tasks;

namespace Codeworx.Identity.Invitation
{
    public interface IInvitationService
    {
        Task<bool> IsSupportedAsync();

        Task<InvitationItem> GetInvitationAsync(string invitationCode);

        Task<InvitationItem> RedeemInvitationAsync(string invitationCode);
    }
}
