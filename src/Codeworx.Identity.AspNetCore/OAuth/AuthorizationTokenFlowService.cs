using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Validation.Authorization;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationTokenFlowService : IAuthorizationFlowService
    {
        private readonly IClientService _oAuthClientService;
        private readonly IScopeService _scopeService;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Token;

        public AuthorizationTokenFlowService(IClientService oAuthClientService, IScopeService scopeService, IEnumerable<ITokenProvider> tokenProviders)
        {
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
            _tokenProviders = tokenProviders;
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request)
        {
            var client = await _oAuthClientService.GetById(request.ClientId);
            if (client == null)
            {
                return new InvalidRequestResult(new ClientIdInvalidResult(request.State));
            }

            if (!client.SupportedFlow.Any(p => p.IsSupported(request.ResponseType)))
            {
                return new UnauthorizedClientResult(request.State, request.RedirectUri);
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
                return new UnknownScopeResult(request.State, request.RedirectUri);
            }

            var provider = _tokenProviders.First(p => p.TokenType == "jwt");
            var token = await provider.CreateAsync(null, DateTime.Now.AddHours(1));
            token.SetPayload(new IdentityData("asdf", "admin", Enumerable.Empty<TenantInfo>(), Enumerable.Empty<AssignedClaim>(), "abcd"), TokenType.AccessToken);
            var accessToken = token.Serialize();

            return new SuccessfulTokenAuthorizationResult(request.State, accessToken, request.RedirectUri);
        }
    }
}