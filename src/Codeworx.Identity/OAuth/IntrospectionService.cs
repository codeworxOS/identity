using System;
using System.Threading;
using System.Threading.Tasks;
using Codeworx.Identity.Model;
using Codeworx.Identity.Response;
using Codeworx.Identity.Token;
using Microsoft.Extensions.Logging;

namespace Codeworx.Identity.OAuth
{
    // EventIds 154xx
    public partial class IntrospectionService : IIntrospectionService
    {
        private readonly IClientAuthenticationService _clientAuthenticationService;
        private readonly ITokenProviderService _tokenProviderService;
        private readonly ILogger<IntrospectionService> _logger;

        public IntrospectionService(IClientAuthenticationService clientAuthenticationService, ITokenProviderService tokenProviderService, ILogger<IntrospectionService> logger)
        {
            _clientAuthenticationService = clientAuthenticationService;
            _tokenProviderService = tokenProviderService;
            _logger = logger;
        }

        [LoggerMessage(
        EventId = 15401,
        Level = LogLevel.Warning,
        Message = "Client could not be authenticated.")]
        public static partial void LogClientNotAuthenticated(ILogger logger, Exception ex);

        [LoggerMessage(
        EventId = 15402,
        Level = LogLevel.Warning,
        Message = "The client_id {clientId} has a ClientType that is not allowed to access the introspection endpoint.")]
        public static partial void LogClientNotAllowed(ILogger logger, string clientId);

        public async Task<IIntrospectResponse> ProcessAsync(IntrospectRequest request, CancellationToken token = default)
        {
            IClientRegistration client = null;
            try
            {
                client = await _clientAuthenticationService.AuthenticateClient(request.ClientId, request.ClientSecret);
            }
            catch (Exception ex)
            {
                LogClientNotAuthenticated(_logger, ex);

                throw new ErrorResponseException<UnauthorizedResponse>(new UnauthorizedResponse());
            }

            if (!client.ClientType.HasFlag(ClientType.Backend))
            {
                LogClientNotAllowed(_logger, client.ClientId);
                return new IntrospectResponse(false);
            }

            var tokenFormat = _tokenProviderService.GetTokenFormat(request.Token);
            var accessToken = await _tokenProviderService.CreateTokenAsync(tokenFormat, TokenType.AccessToken, null, token).ConfigureAwait(false);
            await accessToken.ParseAsync(request.Token, token).ConfigureAwait(false);

            var validUntil = accessToken.ValidUntil;
            return new IntrospectResponse(validUntil, accessToken.IdentityData.GetTokenClaims(ClaimTarget.ProfileEndpoint));
        }
    }
}
