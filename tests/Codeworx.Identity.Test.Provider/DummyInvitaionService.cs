using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Invitation;

namespace Codeworx.Identity.Test.Provider
{
    public class DummyInvitaionService : IInvitationService
    {
        private readonly Dictionary<string, InvitationItem> _invitations;

        private readonly Dictionary<string, InvitationItem> _redeemed;


        public DummyInvitaionService()
        {
            _redeemed = new Dictionary<string, InvitationItem>();
            _invitations = new Dictionary<string, InvitationItem>() {
                {
                    TestConstants.Invitations.Default.Code,
                    new InvitationItem{
                        Action = InvitationAction.All,
                        RedirectUri = TestConstants.Invitations.Default.ReturnUrl,
                        UserId = TestConstants.Users.DefaultAdmin.UserId,
                    }
                },
            };
        }

        public Task<InvitationItem> GetInvitationAsync(string invitationCode)
        {
            if (_invitations.TryGetValue(invitationCode, out var item))
            {
                return Task.FromResult(item);
            }

            throw new InvitationNotFoundException();
        }

        public Task<bool> IsSupportedAsync()
        {
            return Task.FromResult(true);
        }

        public Task<InvitationItem> RedeemInvitationAsync(string invitationCode)
        {
            if (_invitations.TryGetValue(invitationCode, out var item))
            {
                _invitations.Remove(invitationCode);
                _redeemed.Add(invitationCode, item);
                return Task.FromResult(item);
            }
            else if (_redeemed.ContainsKey(invitationCode))
            {
                throw new InvitationAlreadyRedeemedException();
            }

            throw new InvitationNotFoundException();
        }
    }
}
