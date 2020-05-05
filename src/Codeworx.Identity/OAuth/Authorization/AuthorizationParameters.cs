using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationParameters : IAuthorizationParameters
    {
        public AuthorizationParameters(
            string clientId,
            string nonce,
            string redirectUri,
            string responseMode,
            IEnumerable<string> responseTypes,
            IEnumerable<string> scopes,
            string state,
            ClaimsIdentity user,
            AuthorizationRequest request)
        {
            ClientId = clientId;
            Nonce = nonce;
            RedirectUri = redirectUri;
            ResponseMode = responseMode;
            ResponseTypes = ImmutableArray.CreateRange(responseTypes);
            Scopes = ImmutableArray.CreateRange(scopes);
            State = state;
            User = user;
            Request = request;
        }

        public string ClientId { get; }

        public string Nonce { get; }

        public string RedirectUri { get; }

        public string ResponseMode { get; }

        public IEnumerable<string> ResponseTypes { get; }

        public IEnumerable<string> Scopes { get; }

        public string State { get; }

        public ClaimsIdentity User { get; }

        public AuthorizationRequest Request { get; }
    }
}
