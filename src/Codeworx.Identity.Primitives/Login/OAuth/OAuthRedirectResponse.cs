using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectResponse
    {
        public OAuthRedirectResponse(string authorizationEndpoint, string clientId, string state, string nonce, string callbackUri, IEnumerable<string> scopes, string prompt)
        {
            AuthorizationEndpoint = authorizationEndpoint;
            Scopes = scopes.ToImmutableList();
            ClientId = clientId;
            State = state;
            Nonce = nonce;
            CallbackUri = callbackUri;
            Prompt = prompt;
        }

        public string AuthorizationEndpoint { get; }

        public IReadOnlyList<string> Scopes { get; }

        public string ClientId { get; }

        public string State { get; }

        public string Nonce { get; set; }

        public string CallbackUri { get; }

        public string Prompt { get; }
    }
}
