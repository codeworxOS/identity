using System;
using System.Collections.Generic;
using System.Linq;
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

        public Task<string> CreateAccessToken(IDictionary<string, string> cacheData, TimeSpan expiresIn)
        {
            return this.CreateToken(cacheData, expiresIn, "jwt", ClaimTarget.AccessToken);
        }

        public Task<string> CreateIdToken(IDictionary<string, string> cacheData, TimeSpan expiresIn)
        {
            return this.CreateToken(cacheData, expiresIn, "jwt", ClaimTarget.IdToken);
        }

        private async Task<string> CreateToken(IDictionary<string, string> cacheData, TimeSpan expiresIn, string tokenType, ClaimTarget target)
        {
            if (cacheData == null)
            {
                throw new ArgumentNullException();
            }

            if (cacheData.ContainsKey(Identity.OAuth.Constants.ClientIdName) == false
                || cacheData.ContainsKey(Identity.OAuth.Constants.RedirectUriName) == false)
            {
                return null;
            }

            if (!cacheData.TryGetValue(Constants.LoginClaimType, out var login))
            {
                return null;
            }

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == tokenType);
            if (tokenProvider == null)
            {
                return null;
            }

            var token = await tokenProvider.CreateAsync(null);

            var identityData = await _identityService.GetIdentityAsync(login);
            var payload = identityData.GetTokenClaims(target);

            await token.SetPayloadAsync(payload, expiresIn);

            return await token.SerializeAsync();
        }
    }
}