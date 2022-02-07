using System.Collections.Generic;
using System.Collections.Immutable;

namespace Codeworx.Identity.Login.OAuth
{
    public class OAuthRedirectResponse
    {
        public OAuthRedirectResponse(string authorizationEndpoint, string clientId, string state, string nonce, string callbackUri, IEnumerable<string> scopes, string prompt, Dictionary<string, object> authrizationParameters)
        {
            AuthorizationEndpoint = authorizationEndpoint;
            Scopes = scopes.ToImmutableList();
            ClientId = clientId;
            State = state;
            Nonce = nonce;
            CallbackUri = callbackUri;
            Prompt = prompt;
            AuthrizationParameters = authrizationParameters;
        }

        public string AuthorizationEndpoint { get; }

        public IReadOnlyList<string> Scopes { get; }

        public string ClientId { get; }

        public string State { get; }

        public string Nonce { get; set; }

        public string CallbackUri { get; }

        public string Prompt { get; }

        public Dictionary<string, object> AuthrizationParameters { get; }
    }
}
