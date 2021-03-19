using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.Invitation
{
    // EventIds 144xx
    public class InvitationService : IInvitationService
    {
        private static readonly Action<ILogger, Exception> _getIvitationLogMessage;
        private static readonly Action<ILogger, string, Exception> _getIvitationParameterLogMessage;
        private static readonly Action<ILogger, Exception> _redeemIvitationLogMessage;
        private static readonly Action<ILogger, string, Exception> _redeemIvitationParameterLogMessage;
        private readonly ILogger<InvitationService> _logger;
        private readonly IInvitationCache _cache;

        static InvitationService()
        {
            _getIvitationLogMessage = LoggerMessage.Define(LogLevel.Information, new EventId(14401), nameof(GetInvitationAsync));
            _getIvitationParameterLogMessage = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(14402), $"{nameof(GetInvitationAsync)}: {{invitationCode}}");

            _redeemIvitationLogMessage = LoggerMessage.Define(LogLevel.Information, new EventId(14403), nameof(RedeemInvitationAsync));
            _redeemIvitationParameterLogMessage = LoggerMessage.Define<string>(LogLevel.Trace, new EventId(14403), $"{nameof(RedeemInvitationAsync)}: {{invitationCode}}");
        }

        public InvitationService(IInvitationCache cache, ILogger<InvitationService> logger)
        {
            _logger = logger;
            _cache = cache;
        }

        public async Task<InvitationItem> GetInvitationAsync(string invitationCode)
        {
            _getIvitationLogMessage(_logger, null);
            _getIvitationParameterLogMessage(_logger, invitationCode, null);

            return await _cache.GetAsync(invitationCode);
        }

        public async Task<InvitationItem> RedeemInvitationAsync(string invitationCode)
        {
            _redeemIvitationLogMessage(_logger, null);
            _redeemIvitationParameterLogMessage(_logger, invitationCode, null);

            return await _cache.RedeemAsync(invitationCode);
        }
    }
}
