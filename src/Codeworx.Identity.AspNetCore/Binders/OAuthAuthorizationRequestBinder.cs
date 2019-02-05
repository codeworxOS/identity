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
            IReadOnlyCollection<string> clientId = null;
            IReadOnlyCollection<string> redirectUri = null;
            IReadOnlyCollection<string> responseType = null;
            IReadOnlyCollection<string> scope = null;
            IReadOnlyCollection<string> state = null;

            query?.TryGetValue(OAuth.Constants.ClientIdName, out clientId);
            query?.TryGetValue(OAuth.Constants.RedirectUriName, out redirectUri);
            query?.TryGetValue(OAuth.Constants.ResponseTypeName, out responseType);
            query?.TryGetValue(OAuth.Constants.ScopeName, out scope);
            query?.TryGetValue(OAuth.Constants.StateName, out state);

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
