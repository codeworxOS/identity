using System;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    public class ClientCredentialsParametersBuilder : IClientCredentialsParametersBuilder
    {
        private ClaimsIdentity _user;
        private string _nonce;
        private string _state;
        private IClientRegistration _client;
        private string _clientSecret;
        private string[] _scopes;

        public IClientCredentialsParameters Parameters => new ClientCredentialsParameters(_client, _clientSecret, _nonce, _scopes, _state, _user);

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
                case nameof(IClientCredentialsParameters.Client):
                    _client = (IClientRegistration)value;
                    break;
                case nameof(IClientCredentialsParameters.ClientSecret):
                    _clientSecret = (string)value;
                    break;
                case nameof(IClientCredentialsParameters.Scopes):
                    _scopes = (string[])value;
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
