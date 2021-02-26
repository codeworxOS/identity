using System.Threading.Tasks;

namespace Codeworx.Identity.Invitation
{
    public interface IInvitationService
    {
        Task<string> InviteAsync(string userId, string redirectUri = null);

        Task<InvitationItem> GetInvitationAsync(string invitationCode);

        Task<InvitationItem> RedeemInvitationAsync(string invitationCode);
    }
}
