using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Test
{
    public class OAuthAuthorizationRequestBuilder
    {
        private string _clientId = Constants.DefaultCodeFlowClientId;
        private string _redirectUri = "https://example.org/redirect";
        private string _responseType = Constants.OAuth.ResponseType.Code;
        private string _scope;
        private string _state;
        private string _nonce;

        public OAuthAuthorizationRequestBuilder WithClientId(string value)
        {
            _clientId = value;

            return this;
        }

        public OAuthAuthorizationRequestBuilder WithRedirectUri(string value)
        {
            _redirectUri = value;

            return this;
        }

        public OAuthAuthorizationRequestBuilder WithResponseType(string value)
        {
            _responseType = value;

            return this;
        }

        public OAuthAuthorizationRequestBuilder WithScope(string value)
        {
            _scope = value;

            return this;
        }

        public OAuthAuthorizationRequestBuilder WithState(string value)
        {
            _state = value;

            return this;
        }

        public AuthorizationRequest Build()
        {
            return new AuthorizationRequest(_clientId, _redirectUri, _responseType, _scope, _state, _nonce);
        }
    }
}