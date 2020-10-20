using System;
using Codeworx.Identity.OAuth;
using Codeworx.Identity.OAuth.Token;

namespace Codeworx.Identity.Test
{
    public class TokenRequestBuilder
    {
        private string _clientId = "SomeClientId";
        private string _grantType = Constants.OAuth.GrantType.AuthorizationCode;
        private string _clientSecret = null;
        private string _scopes = null;
        private string _redirectUrl = null;
        private string _code = null;

        public TokenRequestBuilder WithClientId(string value)
        {
            _clientId = value;

            return this;
        }

        public TokenRequestBuilder WithGrantType(string value)
        {
            _grantType = value;

            return this;
        }

        public TokenRequestBuilder WithScopes(string value)
        {
            _scopes = value;

            return this;
        }

        public TokenRequestBuilder WithCode(string value)
        {
            _code = value;

            return this;
        }

        public TokenRequestBuilder WithClientSecret(string value)
        {
            _clientSecret = value;

            return this;
        }

        public TokenRequest Build()
        {
            if (_grantType == Constants.OAuth.GrantType.AuthorizationCode)
            {
                return new AuthorizationCodeTokenRequest(_clientId, _redirectUrl, _code, _clientSecret);
            }
            else if (_grantType == Constants.OAuth.GrantType.ClientCredentials)
            {
                return new ClientCredentialsTokenRequest(_clientId, _clientSecret, _scopes);
            }

            throw new NotSupportedException("Grant type not supported!");

        }
    }
}