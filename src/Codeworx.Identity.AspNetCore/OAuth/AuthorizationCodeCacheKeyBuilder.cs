using System;
using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeCacheKeyBuilder : IAuthorizationCodeCacheKeyBuilder
    {
        public string Get(AuthorizationRequest request, string userIdentifier)
        {
            if (string.IsNullOrWhiteSpace(request?.ClientId))
            {
                throw new ArgumentNullException(nameof(request), $"{nameof(request.ClientId)} is required!");
            }

            if (string.IsNullOrWhiteSpace(userIdentifier))
            {
                throw new ArgumentNullException(nameof(userIdentifier));
            }

            return $"{userIdentifier}_{request.ClientId}";
        }
    }
}
