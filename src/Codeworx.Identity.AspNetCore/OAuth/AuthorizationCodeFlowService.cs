using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService<OAuthAuthorizationRequest>
    {
        private readonly IAuthorizationCodeGenerator<OAuthAuthorizationRequest> _authorizationCodeGenerator;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IClientService _oAuthClientService;
        private readonly IOptions<AuthorizationCodeOptions> _options;
        private readonly IScopeService _scopeService;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator<OAuthAuthorizationRequest> authorizationCodeGenerator, IClientService oAuthClientService, IScopeService scopeService, IOptions<AuthorizationCodeOptions> options, IAuthorizationCodeCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
            _options = options;
            _cache = cache;
        }

        public string[] SupportedResponseTypes { get; } = { Identity.OAuth.Constants.ResponseType.Code };

        public bool IsSupported(string responseType)
        {
            return Equals(Identity.OAuth.Constants.ResponseType.Code, responseType);
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(OAuthAuthorizationRequest request, ClaimsIdentity user)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var client = await _oAuthClientService.GetById(request.ClientId)
                                                  .ConfigureAwait(false);
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
                                       { Constants.LoginClaimType, user.ToIdentityData().Login },
                                       { Identity.OAuth.Constants.ScopeName, request.Scope },
                                   };

            await _cache.SetAsync(authorizationCode, grantInformation, TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds))
                    .ConfigureAwait(false);

            return new SuccessfulCodeAuthorizationResult(request.State, authorizationCode, request.RedirectionTarget);
        }
    }
}