using System.Collections.Generic;
using System.Linq;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Binding.AuthorizationCodeToken;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeTokenRequestBinder : IRequestBinder<AuthorizationCodeTokenRequest, object>
    {
        public IRequestBindingResult<AuthorizationCodeTokenRequest, object> FromQuery(IReadOnlyDictionary<string, IReadOnlyCollection<string>> query)
        {
            IReadOnlyCollection<string> clientId = null;
            IReadOnlyCollection<string> redirectUri = null;
            IReadOnlyCollection<string> code = null;
            IReadOnlyCollection<string> grantType = null;
            IReadOnlyCollection<string> clientSecret = null;

            query?.TryGetValue(Identity.OAuth.Constants.ClientIdName, out clientId);
            query?.TryGetValue(Identity.OAuth.Constants.RedirectUriName, out redirectUri);
            query?.TryGetValue(Identity.OAuth.Constants.CodeName, out code);
            query?.TryGetValue(Identity.OAuth.Constants.GrantTypeName, out grantType);
            query?.TryGetValue(Identity.OAuth.Constants.ClientSecretName, out clientSecret);

            if (clientId?.Count > 1)
            {
                return new ClientIdDuplicatedResult();
            }

            if (redirectUri?.Count > 1)
            {
                return new RedirectUriDuplicatedResult();
            }

            if (code?.Count > 1)
            {
                return new CodeDuplicatedResult();
            }

            if (grantType?.Count > 1)
            {
                return new GrantTypeDuplicatedResult();
            }

            if (clientSecret?.Count > 1)
            {
                return new ClientSecretDuplicatedResult();
            }

            var request = new AuthorizationCodeTokenRequest(clientId?.FirstOrDefault(),
                                                            redirectUri?.FirstOrDefault(),
                                                            code?.FirstOrDefault(),
                                                            grantType?.FirstOrDefault(),
                                                            clientSecret?.FirstOrDefault());

            return new SuccessfulBindingResult(request);
        }
    }
}
