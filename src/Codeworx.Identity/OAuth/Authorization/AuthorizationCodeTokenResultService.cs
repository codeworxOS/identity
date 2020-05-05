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

        public Task<string> CreateAccessToken(IdentityData identityData, TimeSpan expiresIn)
        {
            return this.CreateToken(identityData, expiresIn, "jwt", ClaimTarget.AccessToken);
        }

        public Task<string> CreateIdToken(IdentityData identityData, TimeSpan expiresIn)
        {
            return this.CreateToken(identityData, expiresIn, "jwt", ClaimTarget.IdToken);
        }

        private async Task<string> CreateToken(IdentityData identityData, TimeSpan expiresIn, string tokenType, ClaimTarget target)
        {
            if (identityData == null)
            {
                throw new ArgumentNullException();
            }

            var tokenProvider = _tokenProviders.FirstOrDefault(p => p.TokenType == tokenType);
            if (tokenProvider == null)
            {
                return null;
            }

            var token = await tokenProvider.CreateAsync(null);

            await token.SetPayloadAsync(identityData.GetTokenClaims(target), expiresIn).ConfigureAwait(false);

            return await token.SerializeAsync();
        }
    }
}