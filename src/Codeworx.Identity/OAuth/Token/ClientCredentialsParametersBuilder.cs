using System;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    public class ClientCredentialsParametersBuilder : IClientCredentialsParametersBuilder
    {
        private ClaimsIdentity _user;
        private IUser _identityUser;
        private IClientRegistration _client;
        private string[] _scopes;
        private DateTimeOffset? _validUntil;

        public IClientCredentialsParameters Parameters => _validUntil.HasValue ?
            new ClientCredentialsParameters(_client, _scopes, _user, _identityUser, _validUntil.Value) :
            new ClientCredentialsParameters(_client, _scopes, _user, _identityUser);

        public void SetValue(string property, object value)
        {
            switch (property)
            {
                case nameof(IClientCredentialsParameters.User):
                    _user = (ClaimsIdentity)value;
                    break;
                case nameof(IClientCredentialsParameters.IdentityUser):
                    _identityUser = (IUser)value;
                    break;
                case nameof(IClientCredentialsParameters.Client):
                    _client = (IClientRegistration)value;
                    break;
                case nameof(IClientCredentialsParameters.Scopes):
                    _scopes = (string[])value;
                    break;
                case nameof(IClientCredentialsParameters.TokenValidUntil):
                    _validUntil = (DateTimeOffset)value;
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
