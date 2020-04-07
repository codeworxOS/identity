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
    public class AuthorizationTokenFlowService : IAuthorizationFlowService<AuthorizationRequest>
    {
        private readonly IIdentityService _identityService;
        private readonly IClientService _oAuthClientService;
        private readonly IScopeService _scopeService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IBaseUriAccessor _baseUriAccessor;

        public AuthorizationTokenFlowService(IIdentityService identityService, IClientService oAuthClientService, IScopeService scopeService, IEnumerable<ITokenProvider> tokenProviders, IBaseUriAccessor baseUriAccessor = null)
        {
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
            _tokenProviders = tokenProviders;
            _baseUriAccessor = baseUriAccessor;
            _identityService = identityService;
        }

        public string[] SupportedResponseTypes { get; } = { Identity.OAuth.Constants.ResponseType.Token };

        public bool IsSupported(string responseType)
        {
            return Equals(Identity.OAuth.Constants.ResponseType.Token, responseType);
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, ClaimsIdentity user)
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
            var issuer = _baseUriAccessor?.BaseUri.OriginalString;

            await token.SetPayloadAsync(payload, issuer, request.ClientId, user, request.Scope, request.Nonce, client.TokenExpiration).ConfigureAwait(false);

            var accessToken = await token.SerializeAsync().ConfigureAwait(false);

            return new SuccessfulTokenAuthorizationResult(request.State, accessToken, Convert.ToInt32(Math.Floor(client.TokenExpiration.TotalSeconds)), request.RedirectionTarget);
        }
    }
}