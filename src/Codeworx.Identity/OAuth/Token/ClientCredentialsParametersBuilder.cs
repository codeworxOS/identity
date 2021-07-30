using System;
using System.Security.Claims;

namespace Codeworx.Identity.OAuth.Token
{
    public class ClientCredentialsParametersBuilder : IClientCredentialsParametersBuilder
    {
        private ClaimsIdentity _user;
        private string _nonce;
        private string _state;
        private string _clientId;
        private string _clientSecret;
        private string[] _scopes;
        private TimeSpan _tokenExpiration;

        public IClientCredentialsParameters Parameters => new ClientCredentialsParameters(_clientId, _clientSecret, _nonce, _scopes, _state, _tokenExpiration, _user);

        public void SetValue(string property, object value)
        {
            switch (property)
            {
                case nameof(IClientCredentialsParameters.User):
                    _user = (ClaimsIdentity)value;
                    break;
                case nameof(IClientCredentialsParameters.Nonce):
                    _nonce = (string)value;
                    break;
                case nameof(IClientCredentialsParameters.State):
                    _state = (string)value;
                    break;
                case nameof(IClientCredentialsParameters.ClientId):
                    _clientId = (string)value;
                    break;
                case nameof(IClientCredentialsParameters.ClientSecret):
                    _clientSecret = (string)value;
                    break;
                case nameof(IClientCredentialsParameters.Scopes):
                    _scopes = (string[])value;
                    break;
                case nameof(IClientCredentialsParameters.TokenExpiration):
                    _tokenExpiration = (TimeSpan)value;
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
