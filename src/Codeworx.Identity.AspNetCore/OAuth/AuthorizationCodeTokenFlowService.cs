using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenFlowService : ITokenFlowService
    {
        private readonly IDistributedCache _cache;

        public string SupportedGrantType => Identity.OAuth.Constants.GrantType.AuthorizationCode;

        public AuthorizationCodeTokenFlowService(IDistributedCache cache)
        {
            _cache = cache;
        }

        public async Task<ITokenResult> AuthorizeRequest(TokenRequest request)
        {
            if (request == null || !(request is AuthorizationCodeTokenRequest authorizationCodeTokenRequest))
            {
                throw new ArgumentNullException(nameof(request));
            }

            var cachedGrantInformation = await _cache.GetStringAsync(authorizationCodeTokenRequest.Code)
                                                     .ConfigureAwait(false);
            if (string.IsNullOrWhiteSpace(cachedGrantInformation))
            {
                return new InvalidGrantResult();
            }

            var deserializedGrantInformation = JsonConvert.DeserializeObject<Dictionary<string, string>>(cachedGrantInformation);
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
