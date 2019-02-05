using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Exceptions;

namespace Codeworx.Identity.AspNetCore.Binders
{
    public class OAuthAuthorizationRequestBinder : IRequestBinder<AuthorizationRequest>
    {
        public AuthorizationRequest FromQuery(IReadOnlyDictionary<string, IReadOnlyCollection<string>> query)
        {
            query.TryGetValue(OAuth.Constants.ClientIdName, out var clientId);
            query.TryGetValue(OAuth.Constants.RedirectUriName, out var redirectUri);
            query.TryGetValue(OAuth.Constants.ResponseTypeName, out var responseType);
            query.TryGetValue(OAuth.Constants.ScopeName, out var scope);
            query.TryGetValue(OAuth.Constants.StateName, out var state);

            if (clientId?.Count > 1)
            {
                throw new ClientIdDuplicatedException(state?.FirstOrDefault());
            }

            if (redirectUri?.Count > 1)
            {
                throw new RedirectUriDuplicatedException(state?.FirstOrDefault());
            }

            if (responseType?.Count > 1)
            {
                throw new ResponseTypeDuplicatedException(state?.FirstOrDefault());
            }

            if (scope?.Count > 1)
            {
                throw new ScopeDuplicatedException(state?.FirstOrDefault());
            }

            if (state?.Count > 1)
            {
                throw new StateDuplicatedException(state?.FirstOrDefault());
            }

            return new AuthorizationRequest(clientId?.FirstOrDefault(),
                                            redirectUri?.FirstOrDefault(),
                                            responseType?.FirstOrDefault(),
                                            scope?.FirstOrDefault(),
                                            state?.FirstOrDefault());
        }
    }
}
