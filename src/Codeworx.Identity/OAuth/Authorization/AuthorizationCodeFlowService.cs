﻿using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IIdentityService _identityService;
        private readonly IClientService _clientService;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IOptions<AuthorizationCodeOptions> _options;

        public AuthorizationCodeFlowService(
            IAuthorizationCodeGenerator authorizationCodeGenerator,
            IClientService clientService,
            IIdentityService identityService,
            IOptions<AuthorizationCodeOptions> options,
            IAuthorizationCodeCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _identityService = identityService;
            _clientService = clientService;
            _options = options;
            _cache = cache;
        }

        public string[] SupportedResponseTypes { get; } = { Constants.OAuth.ResponseType.Code };

        public bool IsSupported(string responseType)
        {
            return Equals(Constants.OAuth.ResponseType.Code, responseType);
        }

        public async Task<IAuthorizationResult> AuthorizeRequest(IAuthorizationParameters parameters)
        {
            if (parameters == null)
            {
                throw new ArgumentNullException(nameof(parameters));
            }

            var client = await _clientService.GetById(parameters.ClientId)
                                                  .ConfigureAwait(false);
            if (client == null)
            {
                return InvalidRequestResult.CreateInvalidClientId(parameters.State);
            }

            // TODO ClientType check
            ////if (!client.SupportedFlow.Any(p => p.IsSupported(request.ResponseType)))
            ////{
            ////    return new UnauthorizedClientResult(request.State, request.RedirectionTarget);
            ////}

            var authorizationCode = await _authorizationCodeGenerator.GenerateCode(parameters, _options.Value.Length)
                                                                     .ConfigureAwait(false);

            var identity = await _identityService.GetIdentityAsync(parameters).ConfigureAwait(false);

            await _cache.SetAsync(authorizationCode, identity, TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds))
                    .ConfigureAwait(false);

            return new SuccessfulCodeAuthorizationResult(parameters.State, authorizationCode, parameters.RedirectUri);
        }
    }
}