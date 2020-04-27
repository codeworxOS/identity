using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Test
{
    public class TokenRequestBuilder
    {
        private class MockTokenRequest : TokenRequest
        {
            public MockTokenRequest(string clientId, string redirectUri, string grantType, string clientSecret)
                : base(clientId, redirectUri, grantType, clientSecret) { }
        }

        private string _clientId = "SomeClientId";
        private string _redirectUri = "http://example.org/redirect";
        private string _grantType = Constants.OAuth.GrantType.AuthorizationCode;
        private string _clientSecret = string.Empty;

        public TokenRequestBuilder WithClientId(string value)
        {
            _clientId = value;

            return this;
        }

        public TokenRequestBuilder WithRedirectUri(string value)
        {
            _redirectUri = value;

            return this;
        }

        public TokenRequestBuilder WithGrantType(string value)
        {
            _grantType = value;

            return this;
        }

        public TokenRequestBuilder WithClientSecret(string value)
        {
            _clientSecret = value;

            return this;
        }

        public TokenRequest Build()
        {
            return new MockTokenRequest(_clientId, _redirectUri, _grantType, _clientSecret);
        }
    }
}