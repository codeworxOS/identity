using System;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationResponseBuilder : IAuthorizationResponseBuilder
    {
        private string _state;
        private string _code;
        private string _token;
        private int? _expiresIn;
        private string _identityToken;
        private string _redirectUri;

        public AuthorizationSuccessResponse Response => new AuthorizationSuccessResponse(_state, _code, _token, _expiresIn, _identityToken, _redirectUri);

        public void RaiseError(string error)
        {
            AuthorizationErrorResponse.Throw(error, null, _state, _redirectUri);
        }

        public IAuthorizationResponseBuilder WithAccessToken(string accessToken, TimeSpan tokenExpiration)
        {
            _token = accessToken;
            _expiresIn = Convert.ToInt32(tokenExpiration.TotalSeconds);
            return this;
        }

        public IAuthorizationResponseBuilder WithCode(string authorizationCode)
        {
            _code = authorizationCode;
            return this;
        }

        public IAuthorizationResponseBuilder WithIdToken(string identityToken)
        {
            _identityToken = identityToken;
            return this;
        }

        public IAuthorizationResponseBuilder WithRedirectUri(string redirectUri)
        {
            _redirectUri = redirectUri;
            return this;
        }

        public IAuthorizationResponseBuilder WithState(string state)
        {
            _state = state;
            return this;
        }
    }
}
