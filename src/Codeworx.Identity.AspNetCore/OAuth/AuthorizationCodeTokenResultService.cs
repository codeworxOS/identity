using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenResultService : ITokenResultService
    {
        private readonly IAuthorizationCodeCache _cache;
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IClientService _clientService;
        private readonly IIdentityService _identityService;

        public AuthorizationCodeTokenResultService(IAuthorizationCodeCache cache, IIdentityService identityService, IClientService clientService, IEnumerable<ITokenProvider> tokenProviders)
        {
            _cache = cache;
            _tokenProviders = tokenProviders;
            _clientService = clientService;
            _identityService = identityService;
        }

        public string SupportedGrantType => Identity.OAuth.Constants.GrantType.AuthorizationCode;

        public async Task<string> CreateAccessToken(Dictionary<string, string> cacheData, ClaimsIdentity user)
        {
            if (cacheData == null)
            {
                throw new ArgumentNullException();
            }

            if (user == null)
            {
                throw new ArgumentNullException();
            }

            if (cacheData.ContainsKey(Identity.OAuth.Constants.ClientIdName) == false
                || cacheData.ContainsKey(Identity.OAuth.Constants.RedirectUriName) == false)
            {
                return null;
            }

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == "jwt");
            if (tokenProvider == null)
            {
                return null;
            }

            var token = await tokenProvider.CreateAsync(null);

            var identityData = await _identityService.GetIdentityAsync(user);
            var payload = identityData.GetTokenClaims(ClaimTarget.AccessToken);

            var client = await _clientService.GetById(cacheData[Identity.OAuth.Constants.ClientIdName]);

            await token.SetPayloadAsync(payload, client.TokenExpiration);

            return await token.SerializeAsync();
        }

        public async Task<ITokenResult> ProcessRequest(TokenRequest request)
        {
            var authorizationCodeTokenRequest = request as AuthorizationCodeTokenRequest;

            if (request == null || authorizationCodeTokenRequest == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            var deserializedGrantInformation = await _cache.GetAsync(authorizationCodeTokenRequest.Code)
                                                     .ConfigureAwait(false);

            if (deserializedGrantInformation == null
                || !deserializedGrantInformation.TryGetValue(Identity.OAuth.Constants.RedirectUriName, out var redirectUri)
                || redirectUri != request.RedirectUri
                || !deserializedGrantInformation.TryGetValue(Identity.OAuth.Constants.ClientIdName, out var clientId)
                || clientId != request.ClientId)
            {
                return new InvalidGrantResult();
            }

            return new SuccessfulTokenResult("TokenGenerationNotImplemented", Identity.OAuth.Constants.TokenType.Bearer);
        }
    }
}