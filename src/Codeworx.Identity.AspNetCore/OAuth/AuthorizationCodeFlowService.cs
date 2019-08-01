﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Authorization;
using Codeworx.Identity.OAuth.Validation.Authorization;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IClientService _oAuthClientService;
        private readonly IScopeService _scopeService;
        private readonly IOptions<AuthorizationCodeOptions> _options;
        private readonly IDistributedCache _cache;

        public string SupportedAuthorizationResponseType => Identity.OAuth.Constants.ResponseType.Code;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator authorizationCodeGenerator, IClientService oAuthClientService, IScopeService scopeService, IOptions<AuthorizationCodeOptions> options, IDistributedCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _oAuthClientService = oAuthClientService;
            _scopeService = scopeService;
            _options = options;
            _cache = cache;
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
                          .Any(p => !scopeKeys.Contains(p)) == true)
            {
                return new UnknownScopeResult(request.State, request.RedirectUri);
            }

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request)
                                                                     .ConfigureAwait(false);

            var grantInformation = new Dictionary<string, string>
                                   {
                                       {Identity.OAuth.Constants.RedirectUriName, request.RedirectUri},
                                       {Identity.OAuth.Constants.ClientIdName, request.ClientId}
                                   };

            await _cache.SetStringAsync(authorizationCode,
                                        JsonConvert.SerializeObject(grantInformation),
                                        new DistributedCacheEntryOptions
                                        {
                                            AbsoluteExpirationRelativeToNow = TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds)
                                        })
                        .ConfigureAwait(false);

            return new SuccessfulCodeAuthorizationResult(request.State, authorizationCode, request.RedirectUri);
        }
    }
}