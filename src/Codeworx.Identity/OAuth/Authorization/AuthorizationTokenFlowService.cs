using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationTokenFlowService : IAuthorizationFlowService
    {
        private readonly IIdentityService _identityService;
        private readonly IClientService _clientService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public AuthorizationTokenFlowService(IIdentityService identityService, IClientService clientService, IEnumerable<ITokenProvider> tokenProviders, IBaseUriAccessor baseUriAccessor = null)
        {
            _clientService = clientService;
            _tokenProviders = tokenProviders;
            _baseUriAccessor = baseUriAccessor;
            _identityService = identityService;
        }

        public string[] SupportedResponseTypes { get; } = { Constants.OAuth.ResponseType.Token };

        public bool IsSupported(string responseType)
        {
            return Equals(Constants.OAuth.ResponseType.Token, responseType);
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(IAuthorizationParameters parameters)
        {
            var client = await _clientService.GetById(parameters.ClientId);
            if (client == null)
            {
                return InvalidRequestResult.CreateInvalidClientId(parameters.State);
            }

            // TODO implement ClientType
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.ResponseType)))
            ////{
            ////    return new UnauthorizedClientResult(request.State, request.RedirectionTarget);
            ////}

            var provider = _tokenProviders.First(p => p.TokenType == "jwt");
            var token = await provider.CreateAsync(null);

            var identityData = await _identityService.GetIdentityAsync(parameters);
            var payload = identityData.GetTokenClaims(ClaimTarget.AccessToken);
            var issuer = _baseUriAccessor?.BaseUri.OriginalString;

            await token.SetPayloadAsync(payload, client.TokenExpiration).ConfigureAwait(false);

            var accessToken = await token.SerializeAsync().ConfigureAwait(false);

            return new SuccessfulTokenAuthorizationResult(parameters.State, accessToken, Convert.ToInt32(Math.Floor(client.TokenExpiration.TotalSeconds)), parameters.RedirectUri);
        }
    }
}