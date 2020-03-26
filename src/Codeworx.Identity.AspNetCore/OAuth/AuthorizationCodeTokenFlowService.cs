using System;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenFlowService : ITokenFlowService
    {
        private readonly IAuthorizationCodeCache _cache;

        public AuthorizationCodeTokenFlowService(IAuthorizationCodeCache cache)
        {
            _cache = cache;
        }

        public string SupportedGrantType => Identity.OAuth.Constants.GrantType.AuthorizationCode;

        public async Task<ITokenResult> AuthorizeRequest(TokenRequest request)
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