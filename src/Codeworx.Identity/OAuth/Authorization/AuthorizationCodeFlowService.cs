using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService
    {
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IAuthorizationCodeCache _cache;
        private readonly IClientService _clientService;
        private readonly IOptions<AuthorizationCodeOptions> _options;

        public AuthorizationCodeFlowService(
            IAuthorizationCodeGenerator authorizationCodeGenerator,
            IClientService clientService,
            IOptions<AuthorizationCodeOptions> options,
            IAuthorizationCodeCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
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

            var grantInformation = new Dictionary<string, string>
                                   {
                                       { Constants.OAuth.RedirectUriName, parameters.RedirectUri },
                                       { Constants.OAuth.ClientIdName, parameters.ClientId },
                                       { Constants.Claims.Name, parameters.User.ToIdentityData().Login },
                                       { Constants.OAuth.ScopeName, string.Join(" ", parameters.Scopes) },
                                   };

            await _cache.SetAsync(authorizationCode, grantInformation, TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds))
                    .ConfigureAwait(false);

            return new SuccessfulCodeAuthorizationResult(parameters.State, authorizationCode, parameters.RedirectUri);
        }
    }
}