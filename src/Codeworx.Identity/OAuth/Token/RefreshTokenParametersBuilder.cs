using System;
using System.Security.Claims;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.OAuth.Token
{
    internal class RefreshTokenParametersBuilder : IRefreshTokenParametersBuilder
    {
        private IClientRegistration _client;
        private string _refreshToken;
        private ClaimsIdentity _user;
        private string[] _scopes;
        private IToken _parsedRefreshToken;

        public RefreshTokenParametersBuilder()
        {
        }

        public IRefreshTokenParameters Parameters => new RefreshTokenParameters(_client, _refreshToken, _scopes, _user, _parsedRefreshToken);

        public void SetValue(string property, object value)
        {
            switch (property)
            {
                case nameof(IRefreshTokenParameters.User):
                    _user = (ClaimsIdentity)value;
                    break;
                case nameof(IRefreshTokenParameters.RefreshToken):
                    _refreshToken = (string)value;
                    break;
                case nameof(IRefreshTokenParameters.Client):
                    _client = (IClientRegistration)value;
                    break;
                case nameof(IRefreshTokenParameters.Scopes):
                    _scopes = (string[])value;
                    break;
                case nameof(IRefreshTokenParameters.ParsedRefreshToken):
                    _parsedRefreshToken = (IToken)value;
                    break;
                default:
                    throw new NotSupportedException($"Property {property} not supported!");
            }
        }

        public void Throw(string error, string errorDescription)
        {
            ErrorResponse.Throw(error, errorDescription);
        }
    }
}