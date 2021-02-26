using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.Invitation
{
    public class InvitationService : IInvitationService
    {
        private static readonly Action<ILogger, string, Exception> _alreadyRedeemed;
        private readonly ILogger<InvitationService> _logger;
        private readonly IInvitationCache _cache;

        static InvitationService()
        {
            _alreadyRedeemed = LoggerMessage.Define<string>(LogLevel.Warning, new EventId(15201), "The invitation {Key}, was already redeemed.");
        }

        public InvitationService(IInvitationCache cache, ILogger<InvitationService> logger)
        {
            _logger = logger;
            _cache = cache;
        }

        public Task<InvitationItem> GetInvitationAsync(string invitationCode)
        {
            throw new NotSupportedException();
        }

        public Task<string> InviteAsync(string userId, string redirectUri = null)
        {
            throw new NotSupportedException();
        }

        public Task<InvitationItem> RedeemInvitationAsync(string invitationCode)
        {
            throw new NotSupportedException();
        }
    }
}
