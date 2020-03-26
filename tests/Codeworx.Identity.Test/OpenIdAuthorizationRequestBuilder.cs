using Codeworx.Identity.OpenId;

namespace Codeworx.Identity.Test
{
    public class OpenIdAuthorizationRequestBuilder
    {
        private string _clientId = Constants.DefaultCodeFlowClientId;
        private string _redirectUri = "https://example.org/redirect";
        private string _responseType = OAuth.Constants.ResponseType.Code;
        private string _scope;
        private string _state;
        private string _nonce = "ajhsdfojhplosjhdfio0hopishfpozhsd8ufhij";

        public OpenIdAuthorizationRequestBuilder WithClientId(string value)
        {
            _clientId = value;

            return this;
        }

        public OpenIdAuthorizationRequestBuilder WithRedirectUri(string value)
        {
            _redirectUri = value;

            return this;
        }

        public OpenIdAuthorizationRequestBuilder WithResponseType(string value)
        {
            _responseType = value;

            return this;
        }

        public OpenIdAuthorizationRequestBuilder WithScope(string value)
        {
            _scope = value;

            return this;
        }

        public OpenIdAuthorizationRequestBuilder WithState(string value)
        {
            _state = value;

            return this;
        }

        public OpenIdAuthorizationRequestBuilder WithNonce(string value)
        {
            _nonce = value;

            return this;
        }

        public OpenIdAuthorizationRequest Build()
        {
            return new OpenIdAuthorizationRequest(_clientId, _redirectUri, _responseType, _scope, _state, _nonce);
        }
    }
}