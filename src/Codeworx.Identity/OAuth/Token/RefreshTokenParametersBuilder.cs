using System;
using System.Security.Claims;
using Codeworx.Identity.Cache;

namespace Codeworx.Identity.OAuth.Token
{
    internal class RefreshTokenParametersBuilder : IRefreshTokenParametersBuilder
    {
        private string _clientId;
        private string _clientSecret;
        private string _refreshToken;
        private ClaimsIdentity _user;
        private string[] _scopes;
        private IRefreshTokenCacheItem _cacheItem;
        private TimeSpan _tokenExpiration;

        public RefreshTokenParametersBuilder()
        {
        }

        public IRefreshTokenParameters Parameters => new RefreshTokenParameters(_clientId, _clientSecret, _refreshToken, _scopes, _user, _cacheItem, _tokenExpiration);

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
                case nameof(IRefreshTokenParameters.ClientId):
                    _clientId = (string)value;
                    break;
                case nameof(IRefreshTokenParameters.ClientSecret):
                    _clientSecret = (string)value;
                    break;
                case nameof(IRefreshTokenParameters.Scopes):
                    _scopes = (string[])value;
                    break;
                case nameof(IRefreshTokenParameters.CacheItem):
                    _cacheItem = (IRefreshTokenCacheItem)value;
                    break;
                case nameof(IRefreshTokenParameters.TokenExpiration):
                    _tokenExpiration = (TimeSpan)value;
                    break;
                case nameof(IRefreshTokenParameters.Nonce):
                case nameof(IRefreshTokenParameters.State):
                    throw new NotSupportedException($"The parameter {property} is not supported for refresh token flow!");
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