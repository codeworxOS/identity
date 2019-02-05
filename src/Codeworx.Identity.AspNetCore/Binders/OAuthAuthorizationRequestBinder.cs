using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.BindingResults;

namespace Codeworx.Identity.AspNetCore.Binders
{
    public class OAuthAuthorizationRequestBinder : IRequestBinder<AuthorizationRequest, AuthorizationErrorResponse>
    {
        public IRequestBindingResult<AuthorizationRequest, AuthorizationErrorResponse> FromQuery(IReadOnlyDictionary<string, IReadOnlyCollection<string>> query)
        {
            query.TryGetValue(OAuth.Constants.ClientIdName, out var clientId);
            query.TryGetValue(OAuth.Constants.RedirectUriName, out var redirectUri);
            query.TryGetValue(OAuth.Constants.ResponseTypeName, out var responseType);
            query.TryGetValue(OAuth.Constants.ScopeName, out var scope);
            query.TryGetValue(OAuth.Constants.StateName, out var state);

            if (clientId?.Count > 1)
            {
                return new ClientIdDuplicatedResult(state?.FirstOrDefault());
            }

            if (redirectUri?.Count > 1)
            {
                return new RedirectUriDuplicatedResult(state?.FirstOrDefault());
            }

            if (responseType?.Count > 1)
            {
                return new ResponseTypeDuplicatedResult(state?.FirstOrDefault());
            }

            if (scope?.Count > 1)
            {
                return new ScopeDuplicatedResult(state?.FirstOrDefault());
            }

            if (state?.Count > 1)
            {
                return new StateDuplicatedResult(state?.FirstOrDefault());
            }

            var request = new AuthorizationRequest(clientId?.FirstOrDefault(),
                                                   redirectUri?.FirstOrDefault(),
                                                   responseType?.FirstOrDefault(),
                                                   scope?.FirstOrDefault(),
                                                   state?.FirstOrDefault());

            return new SuccessfulBindingResult(request);
        }
    }
}
