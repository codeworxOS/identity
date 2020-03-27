using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenResultService : ITokenResultService
    {
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IClientService _clientService;
        private readonly IIdentityService _identityService;

        public AuthorizationCodeTokenResultService(IIdentityService identityService, IClientService clientService, IEnumerable<ITokenProvider> tokenProviders)
        {
            _tokenProviders = tokenProviders;
            _clientService = clientService;
            _identityService = identityService;
        }

        public string SupportedGrantType => Identity.OAuth.Constants.GrantType.AuthorizationCode;

        public async Task<string> CreateAccessToken(IDictionary<string, string> cacheData, ClaimsIdentity user)
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
    }
}