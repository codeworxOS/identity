using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.Cache;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

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
            if (request == null || !(request is AuthorizationCodeTokenRequest authorizationCodeTokenRequest))
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

            return new SuccessfulTokenResult(null, null);
        }
    }
}