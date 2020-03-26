using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationTokenFlowService : IAuthorizationFlowService<OAuthAuthorizationRequest>
    {
        private readonly IIdentityService _identityService;
        private readonly IClientService _oAuthClientService;
        private readonly IScopeService _scopeService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;

        public AuthorizationTokenFlowService(IIdentityService identityService, IClientService oAuthClientService, IScopeService scopeService, IEnumerable<ITokenProvider> tokenProviders)
        {
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
            _tokenProviders = tokenProviders;
            _identityService = identityService;
        }

        public bool IsSupported(string responseType)
        {
            return Equals(Identity.OAuth.Constants.ResponseType.Token, responseType);
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(OAuthAuthorizationRequest request, ClaimsIdentity user)
        {
            var client = await _oAuthClientService.GetById(request.ClientId);
            if (client == null)
            {
                return InvalidRequestResult.CreateInvalidClientId(request.State);
            }

            if (!client.SupportedFlow.Any(p => p.IsSupported(request.ResponseType)))
            {
                return new UnauthorizedClientResult(request.State, request.RedirectionTarget);
            }

            var scopes = await _scopeService.GetScopes()
                                            .ConfigureAwait(false);

            var scopeKeys = scopes
                            .Select(s => s.ScopeKey)
                            .ToList();

            if (!string.IsNullOrEmpty(request.Scope)
                && request.Scope
                          .Split(' ')
                          .Any(p => scopeKeys.Contains(p) == false))
            {
                return new UnknownScopeResult(request.State, request.RedirectionTarget);
            }

            var provider = _tokenProviders.First(p => p.TokenType == "jwt");
            var token = await provider.CreateAsync(null);

            var identityData = await _identityService.GetIdentityAsync(user);
            var payload = identityData.GetTokenClaims(ClaimTarget.AccessToken);

            await token.SetPayloadAsync(payload, client.TokenExpiration).ConfigureAwait(false);

            var accessToken = await token.SerializeAsync().ConfigureAwait(false);

            // TODO cach accessToken = Key value = identityData.
            return new SuccessfulTokenAuthorizationResult(request.State, accessToken, Convert.ToInt32(Math.Floor(client.TokenExpiration.TotalSeconds)), request.RedirectionTarget);
        }
    }
}