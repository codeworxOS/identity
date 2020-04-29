using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationCodeTokenResultService : ITokenResultService
    {
        private readonly IEnumerable<ITokenProvider> _tokenProviders;
        private readonly IBaseUriAccessor _baseUriAccessor;
        private readonly IIdentityService _identityService;

        public AuthorizationCodeTokenResultService(IIdentityService identityService, IEnumerable<ITokenProvider> tokenProviders, IBaseUriAccessor baseUriAccessor)
        {
            _tokenProviders = tokenProviders;
            _baseUriAccessor = baseUriAccessor;
            _identityService = identityService;
        }

        public string SupportedGrantType => Constants.OAuth.GrantType.AuthorizationCode;

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

            if (cacheData.ContainsKey(Constants.OAuth.ClientIdName) == false
                || cacheData.ContainsKey(Constants.OAuth.RedirectUriName) == false)
            {
                return null;
            }

            if (!cacheData.TryGetValue(Constants.Claims.Name, out var login))
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
            var issuer = _baseUriAccessor?.BaseUri.OriginalString;
            var audience = cacheData[Constants.OAuth.ClientIdName];
            cacheData.TryGetValue(Constants.OAuth.ScopeName, out var scope);
            cacheData.TryGetValue(Constants.OAuth.NonceName, out var nonce);

            await token.SetPayloadAsync(payload, issuer, audience, identityData.ToClaimsIdentity(), scope, nonce, expiresIn);

            return await token.SerializeAsync();
        }
    }
}