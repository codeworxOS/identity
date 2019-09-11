using Codeworx.Identity.OAuth;

namespace Codeworx.Identity.Test
{
    public class AuthorizationRequestBuilder
    {
        private string _clientId = Constants.DefaultCodeFlowClientId;
        private string _redirectUri = "https://example.org/redirect";
        private string _responseType = OAuth.Constants.ResponseType.Code;
        private string _scope;
        private string _state;

        public AuthorizationRequestBuilder WithClientId(string value)
        {
            _clientId = value;

            return this;
        }

        public AuthorizationRequestBuilder WithRedirectUri(string value)
        {
            _redirectUri = value;

            return this;
        }

        public AuthorizationRequestBuilder WithResponseType(string value)
        {
            _responseType = value;

            return this;
        }

        public AuthorizationRequestBuilder WithScope(string value)
        {
            _scope = value;

            return this;
        }

        public AuthorizationRequestBuilder WithState(string value)
        {
            _state = value;

            return this;
        }

        public AuthorizationRequest Build()
        {
            return new AuthorizationRequest(_clientId, _redirectUri, _responseType, _scope, _state);
        }
    }
}