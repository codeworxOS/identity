using System.Security.Claims;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationParametersBuilder : IAuthorizationParametersBuilder
    {
        private readonly ClaimsIdentity _user;
        private readonly AuthorizationRequest _request;
        private string _clientId;
        private string _nonce;
        private string _redirectUri;
        private string _responseMode;
        private string[] _responseTypes;
        private string[] _scopes;
        private string _state;

        public AuthorizationParametersBuilder(AuthorizationRequest request, ClaimsIdentity user)
        {
            _user = user;
            _request = request;
            _scopes = new string[] { };
            _responseTypes = new string[] { };
        }

        public IAuthorizationParameters Parameters => new AuthorizationParameters(
             _clientId,
             _nonce,
             _redirectUri,
             _responseMode,
             _responseTypes,
             _scopes,
             _state,
             _user,
             _request);

        public void Throw(string error, string errorDescription)
        {
            var errorResponse = new AuthorizationErrorResponse(error, errorDescription, null, this._state, this._redirectUri, this._responseMode);

            throw new ErrorResponseException<AuthorizationErrorResponse>(errorResponse);
        }

        public IAuthorizationParametersBuilder WithClientId(string clientId)
        {
            _clientId = clientId;
            return this;
        }

        public IAuthorizationParametersBuilder WithNonce(string nonce)
        {
            _nonce = nonce;
            return this;
        }

        public IAuthorizationParametersBuilder WithRedirectUri(string redirectUri)
        {
            _redirectUri = redirectUri;
            return this;
        }

        public IAuthorizationParametersBuilder WithResponseMode(string responseMode)
        {
            _responseMode = responseMode;
            return this;
        }

        public IAuthorizationParametersBuilder WithResponseTypes(params string[] responseTypes)
        {
            _responseTypes = responseTypes;
            return this;
        }

        public IAuthorizationParametersBuilder WithScopes(params string[] scopes)
        {
            _scopes = scopes;
            return this;
        }

        public IAuthorizationParametersBuilder WithState(string state)
        {
            _state = state;
            return this;
        }
    }
}
