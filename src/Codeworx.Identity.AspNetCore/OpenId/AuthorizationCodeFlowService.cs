using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.AspNetCore.OAuth;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OpenId;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OpenId
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService<OpenIdAuthorizationRequest>
    {
        private readonly IClientService _clientService;
        private readonly IScopeService _scopeService;
        private readonly IAuthorizationCodeGenerator<OpenIdAuthorizationRequest> _authorizationCodeGenerator;
        private readonly IOptions<AuthorizationCodeOptions> _options;
        private readonly IAuthorizationCodeCache _cache;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator<OpenIdAuthorizationRequest> authorizationCodeGenerator, IClientService clientService, IScopeService scopeService, IOptions<AuthorizationCodeOptions> options, IAuthorizationCodeCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _clientService = clientService;
            _scopeService = scopeService;
            _options = options;
            _cache = cache;
        }

        public bool IsSupported(string responseType)
        {
            return Equals(responseType, Identity.OAuth.Constants.ResponseType.Code);
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(OpenIdAuthorizationRequest request, ClaimsIdentity user)
        {
            var client = await _clientService.GetById(request.ClientId)
                .ConfigureAwait(false);

            if (client == null)
            {
                return InvalidRequestResult.CreateInvalidClientId(request.State);
            }

            if (!client.SupportedFlow.Any(p => p.IsSupported(request.ResponseType)))
            {
                return new UnauthorizedClientResult(request.State, request.RedirectionTarget);
            }

            var containsOpenId = request.Scope
                .Split(' ')
                .Any(p => p.Equals(Identity.OpenId.Constants.Scopes.OpenId));

            if (containsOpenId == false)
            {
                return new MissingOpenidScopeResult(request.State, request.RedirectionTarget);
            }

            var scopes = await _scopeService.GetScopes()
                .ConfigureAwait(false);

            var scopeKeys = scopes
                .Select(s => s.ScopeKey)
                .ToList();

            scopeKeys.Add(Identity.OpenId.Constants.Scopes.OpenId);

            var containsKey = request.Scope
                                  .Split(' ')
                                  .Any(p => !scopeKeys.Contains(p)) == true;

            if (!string.IsNullOrEmpty(request.Scope) && containsKey)
            {
                return new UnknownScopeResult(request.State, request.RedirectionTarget);
            }

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request, _options.Value.Length)
                .ConfigureAwait(false);

            var grantInformation = new Dictionary<string, string>
            {
                { Identity.OAuth.Constants.RedirectUriName, request.RedirectUri },
                { Identity.OAuth.Constants.ClientIdName, request.ClientId },
                { Identity.OAuth.Constants.NonceName, request.Nonce },
                { Identity.OAuth.Constants.ScopeName, request.Scope },
            };

            await _cache.SetAsync(authorizationCode, grantInformation, TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds))
                .ConfigureAwait(false);

            return new SuccessfulCodeAuthorizationResult(request.State, authorizationCode, request.RedirectionTarget);
        }
    }
}