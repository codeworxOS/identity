using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Security.Claims;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationParameters : IAuthorizationParameters
    {
        private readonly DateTimeOffset _validFrom;

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
            IUser identityUser,
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
            IdentityUser = identityUser;
            Request = request;
            _validFrom = DateTimeOffset.UtcNow;
        }

        public IClientRegistration Client { get; }

        public MfaFlowMode MfaFlowMode => MfaFlowMode.Enabled;

        public string Nonce { get; }

        public IReadOnlyCollection<string> Prompts { get; }

        public string RedirectUri { get; }

        public AuthorizationRequest Request { get; }

        public string ResponseMode { get; }

        public IReadOnlyCollection<string> ResponseTypes { get; }

        public IReadOnlyCollection<string> Scopes { get; }

        public string State { get; }

        public DateTimeOffset TokenValidUntil => _validFrom.Add(Client.TokenExpiration);

        public ClaimsIdentity User { get; }

        public IUser IdentityUser { get; }

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
