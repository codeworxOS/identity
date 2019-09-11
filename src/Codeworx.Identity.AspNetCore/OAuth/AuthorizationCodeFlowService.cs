using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Validation.Authorization;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IClientService _oAuthClientService;
        private readonly IOptions<AuthorizationCodeOptions> _options;
        private readonly IScopeService _scopeService;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator authorizationCodeGenerator, IClientService oAuthClientService, IScopeService scopeService, IOptions<AuthorizationCodeOptions> options, IAuthorizationCodeCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
            _options = options;
            _cache = cache;
        }

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Code;

        public async Task<IAuthorizationResult> AuthorizeRequest(AuthorizationRequest request, IdentityData user)
        {
            var client = await _oAuthClientService.GetById(request.ClientId)
                                                  .ConfigureAwait(false);
            if (client == null)
            {
                return new InvalidRequestResult(new ClientIdInvalidResult(request.State));
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
                          .Any(p => !scopeKeys.Contains(p)) == true)
            {
                return new UnknownScopeResult(request.State, request.RedirectionTarget);
            }

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request, _options.Value.Length)
                                                                     .ConfigureAwait(false);

            var grantInformation = new Dictionary<string, string>
                                   {
                                       { Identity.OAuth.Constants.RedirectUriName, request.RedirectUri },
                                       { Identity.OAuth.Constants.ClientIdName, request.ClientId },
                                   };

            await _cache.SetAsync(authorizationCode, grantInformation, TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds))
                    .ConfigureAwait(false);

            return new SuccessfulCodeAuthorizationResult(request.State, authorizationCode, request.RedirectionTarget);
        }
    }
}