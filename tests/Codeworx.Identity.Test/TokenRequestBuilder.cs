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
        private string _refreshCode = null;
        private string _subjectToken = null;
        private string _actorToken = null;
        private string _audience = null;

        public TokenRequestBuilder WithAudience(string value)
        {
            _audience = value;

            return this;
        }

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

        public TokenRequestBuilder WithSubjectToken(string subjectToken)
        {
            _subjectToken = subjectToken;

            return this;
        }
        public TokenRequestBuilder WithActorToken(string actorToken)
        {
            _actorToken = actorToken;

            return this;
        }

        public TokenRequestBuilder WithRefreshCode(string value)
        {
            _refreshCode = value;

            return this;
        }

        public TokenRequestBuilder WithRedirectUri(string uri)
        {
            _redirectUrl = uri;

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
            else if (_grantType == Constants.OAuth.GrantType.RefreshToken)
            {
                return new RefreshTokenRequest(_clientId, _clientSecret, _refreshCode, _scopes);
            }
            else if (_grantType == Constants.OAuth.GrantType.TokenExchange)
            {
                return new TokenExchangeRequest(_clientId, _clientSecret, _audience, _scopes, _subjectToken, Constants.TokenExchange.TokenType.AccessToken, _actorToken, Constants.TokenExchange.TokenType.AccessToken);
            }

            throw new NotSupportedException("Grant type not supported!");

        }
    }
}