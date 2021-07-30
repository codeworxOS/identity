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
            IReadOnlyCollection<string> responseTypes,
            IReadOnlyCollection<string> scopes,
            IReadOnlyCollection<string> prompts,
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
            Prompts = ImmutableArray.CreateRange(prompts);
            State = state;
            User = user;
            Request = request;
        }

        public string ClientId { get; }

        public string Nonce { get; }

        public IReadOnlyCollection<string> Prompts { get; }

        public string RedirectUri { get; }

        public AuthorizationRequest Request { get; }

        public string ResponseMode { get; }

        public IReadOnlyCollection<string> ResponseTypes { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public string State { get; }

        public ClaimsIdentity User { get; }

        public void Throw(string error, string errorDescription)
        {
            var errorResponse = new AuthorizationErrorResponse(error, errorDescription, null, this.State, this.RedirectUri, this.ResponseMode);

            throw new ErrorResponseException<AuthorizationErrorResponse>(errorResponse);
        }
    }
}
