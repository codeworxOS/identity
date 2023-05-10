using System;
using System.Security.Claims;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.OAuth.Token
{
    public class TokenExchangeParametersBuilder : ITokenExchangeParametersBuilder
    {
        private string _subjectToken;
        private string _subjectTokenType;
        private string _actorToken;
        private string _actorTokenType;
        private ClaimsIdentity _user;
        private IUser _identityUser;
        private IClientRegistration _client;
        private string[] _scopes;
        private string _audience;
        private string[] _requestedTokenTypes;

        public ITokenExchangeParameters Parameters => new TokenExchangeParameters(_client, _scopes, _user, _identityUser, _audience, _subjectToken, _subjectTokenType, _actorToken, _actorTokenType, _requestedTokenTypes);

        public void SetValue(string property, object value)
        {
            switch (property)
            {
                case nameof(ITokenExchangeParameters.User):
                    _user = (ClaimsIdentity)value;
                    break;
                case nameof(ITokenExchangeParameters.IdentityUser):
                    _identityUser = (IUser)value;
                    break;
                case nameof(ITokenExchangeParameters.Client):
                    _client = (IClientRegistration)value;
                    break;
                case nameof(ITokenExchangeParameters.Scopes):
                    _scopes = (string[])value;
                    break;
                case nameof(ITokenExchangeParameters.Audience):
                    _audience = (string)value;
                    break;
                case nameof(ITokenExchangeParameters.SubjectToken):
                    _subjectToken = (string)value;
                    break;
                case nameof(ITokenExchangeParameters.SubjectTokenType):
                    _subjectTokenType = (string)value;
                    break;
                case nameof(ITokenExchangeParameters.ActorToken):
                    _actorToken = (string)value;
                    break;
                case nameof(ITokenExchangeParameters.ActorTokenType):
                    _actorTokenType = (string)value;
                    break;
                case nameof(ITokenExchangeParameters.RequestedTokenTypes):
                    _requestedTokenTypes = (string[])value;
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
