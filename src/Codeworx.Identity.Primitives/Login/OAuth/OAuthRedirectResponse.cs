using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectResponse
    {
        public OAuthRedirectResponse(string authorizationEndpoint, string clientId, string state, string callbackUri, IEnumerable<string> scopes)
        {
            AuthorizationEndpoint = authorizationEndpoint;
            Scopes = scopes.ToImmutableList();
            ClientId = clientId;
            State = state;
            CallbackUri = callbackUri;
        }

        public string AuthorizationEndpoint { get; }

        public IReadOnlyList<string> Scopes { get; set; }

        public string ClientId { get; }

        public string State { get; }

        public string CallbackUri { get; set; }
    }
}
