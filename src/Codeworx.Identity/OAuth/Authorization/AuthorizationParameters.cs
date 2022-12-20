using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationParameters : IAuthorizationParameters
    {
        public AuthorizationParameters(
            IClientRegistration client,
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
            Client = client;
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

        public IClientRegistration Client { get; }

        public string Nonce { get; }

        public IReadOnlyCollection<string> Prompts { get; }

        public string RedirectUri { get; }

        public AuthorizationRequest Request { get; }

        public string ResponseMode { get; }

        public IReadOnlyCollection<string> ResponseTypes { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public string State { get; }

        public ClaimsIdentity User { get; }

        public MfaFlowMode MfaFlowModel => MfaFlowMode.Enabled;

        public void Throw(string error, string errorDescription)
        {
            if (error == Constants.OpenId.Error.MfaAuthenticationRequired)
            {
                throw new ErrorResponseException<MissingMfaResponse>(new MissingMfaResponse(Request, User));
            }

            var errorResponse = new AuthorizationErrorResponse(error, errorDescription, null, this.State, this.RedirectUri, this.ResponseMode);

            throw new ErrorResponseException<AuthorizationErrorResponse>(errorResponse);
        }
    }
}
