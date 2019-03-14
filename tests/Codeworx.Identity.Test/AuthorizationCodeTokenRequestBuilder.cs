using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Test
{
    public class AuthorizationCodeTokenRequestBuilder
    {
        private string _clientId = "SomeClientId";
        private string _redirectUri = "http://example.org/redirect";
        private string _code = "SomeAuthorizationCode";
        private string _grantType = OAuth.Constants.GrantType.AuthorizationCode;
        private string _clientSecret = string.Empty;

        public AuthorizationCodeTokenRequestBuilder WithClientId(string value)
        {
            _clientId = value;

            return this;
        }

        public AuthorizationCodeTokenRequestBuilder WithRedirectUri(string value)
        {
            _redirectUri = value;

            return this;
        }

        public AuthorizationCodeTokenRequestBuilder WithCode(string value)
        {
            _code = value;

            return this;
        }

        public AuthorizationCodeTokenRequestBuilder WithGrantType(string value)
        {
            _grantType = value;

            return this;
        }

        public AuthorizationCodeTokenRequestBuilder WithClientSecret(string value)
        {
            _clientSecret = value;

            return this;
        }

        public AuthorizationCodeTokenRequest Build()
        {
            return new AuthorizationCodeTokenRequest(_clientId, _redirectUri, _code, _grantType, _clientSecret);
        }
    }
}