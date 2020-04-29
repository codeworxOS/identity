using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;
using Microsoft.Extensions.Options;

namespace Codeworx.Identity.OpenId
{
    public class AuthorizationCodeFlowService : IAuthorizationFlowService
    {
        private readonly IClientService _clientService;
        private readonly IScopeService _scopeService;
        private readonly IAuthorizationCodeGenerator _authorizationCodeGenerator;
        private readonly IOptions<AuthorizationCodeOptions> _options;
        private readonly IAuthorizationCodeCache _cache;

        public AuthorizationCodeFlowService(IAuthorizationCodeGenerator authorizationCodeGenerator, IClientService clientService, IScopeService scopeService, IOptions<AuthorizationCodeOptions> options, IAuthorizationCodeCache cache)
        {
            _authorizationCodeGenerator = authorizationCodeGenerator;
            _clientService = clientService;
            _scopeService = scopeService;
            _options = options;
            _cache = cache;
        }

        public string[] SupportedResponseTypes { get; } = { Constants.OAuth.ResponseType.Code };

        public bool IsSupported(string responseType)
        {
            return Equals(responseType, Constants.OAuth.ResponseType.Code);
        }

        public Task<IAuthorizationResult> AuthorizeRequest(IAuthorizationParameters parameters)
        {
            throw new NotSupportedException();

            ////if (request == null)
            ////{
            ////    throw new ArgumentNullException(nameof(request));
            ////}

            ////if (user == null)
            ////{
            ////    throw new ArgumentNullException(nameof(user));
            ////}

            ////var client = await _clientService.GetById(request.ClientId)
            ////    .ConfigureAwait(false);

            ////if (client == null)
            ////{
            ////    return InvalidRequestResult.CreateInvalidClientId(request.State);
            ////}

            ////// TODO implement ClientType
            ////////if (!client.SupportedFlow.Any(p => p.IsSupported(request.ResponseType)))
            ////////{
            ////////    return new UnauthorizedClientResult(request.State, request.RedirectionTarget);
            ////////}

            ////var containsOpenId = request.Scope
            ////    .Split(' ')
            ////    .Any(p => p.Equals(Constants.OpenId.Scopes.OpenId));

            ////if (containsOpenId == false)
            ////{
            ////    return new MissingOpenidScopeResult(request.State, request.RedirectUri);
            ////}

            ////var scopes = await _scopeService.GetScopes()
            ////    .ConfigureAwait(false);

            ////var scopeKeys = scopes
            ////    .Select(s => s.ScopeKey)
            ////    .ToList();

            ////scopeKeys.Add(Constants.OpenId.Scopes.OpenId);

            ////var containsKey = request.Scope
            ////                      .Split(' ')
            ////                      .Any(p => !scopeKeys.Contains(p)) == true;

            ////if (!string.IsNullOrEmpty(request.Scope) && containsKey)
            ////{
            ////    return new UnknownScopeResult(request.State, request.RedirectionTarget);
            ////}

            ////var authorizationCode = await _authorizationCodeGenerator.GenerateCode(request, _options.Value.Length)
            ////    .ConfigureAwait(false);

            ////var grantInformation = new Dictionary<string, string>
            ////{
            ////    { Constants.OAuth.RedirectUriName, request.RedirectUri },
            ////    { Constants.OAuth.ClientIdName, request.ClientId },
            ////    { Constants.OAuth.NonceName, request.Nonce },
            ////    { Constants.OAuth.ScopeName, request.Scope },
            ////    { Constants.Claims.Name, user.ToIdentityData().Login },
            ////};

            ////await _cache.SetAsync(authorizationCode, grantInformation, TimeSpan.FromSeconds(_options.Value.ExpirationInSeconds))
            ////    .ConfigureAwait(false);

            ////return new SuccessfulCodeAuthorizationResult(request.State, authorizationCode, request.RedirectionTarget, request.ResponseMode);
        }
    }
}