using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.Invitation
{
    public class InvitationService : IInvitationService
    {
        private readonly ILogger<InvitationService> _logger;
        private readonly IInvitationCache _cache;

        public InvitationService(IInvitationCache cache, ILogger<InvitationService> logger)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<InvitationItem> GetInvitationAsync(string invitationCode)
        {
            return await _cache.GetAsync(invitationCode);
        }

        public async Task<InvitationItem> RedeemInvitationAsync(string invitationCode)
        {
            return await _cache.RedeemAsync(invitationCode);
        }
    }
}
