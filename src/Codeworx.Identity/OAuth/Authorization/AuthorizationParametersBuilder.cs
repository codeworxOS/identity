using System;
using System.Security.Claims;

namespace Codeworx.Identity.OAuth.Authorization
{
    public class AuthorizationParametersBuilder : IAuthorizationParametersBuilder
    {
        private readonly ClaimsIdentity _user;
        private readonly AuthorizationRequest _request;
        private string _clientId;
        private string _nonce;
        private string _redirectUri;
        private string _responseMode;
        private string[] _responseTypes;
        private string[] _scopes;
        private string _state;
        private string[] _prompts;

        public AuthorizationParametersBuilder(AuthorizationRequest request, ClaimsIdentity user)
        {
            _user = user;
            _request = request ?? throw new System.ArgumentNullException(nameof(request));
            _scopes = new string[] { };
            _responseTypes = new string[] { };
            _prompts = new string[] { };
        }

        public IAuthorizationParameters Parameters => new AuthorizationParameters(
             _clientId,
             _nonce,
             _redirectUri,
             _responseMode,
             _responseTypes,
             _scopes,
             _prompts,
             _state,
             _user,
             _request);

        public void SetValue(string property, object value)
        {
            switch (property)
            {
                case nameof(AuthorizationParameters.ClientId):
                    _clientId = (string)value;
                    break;
                case nameof(AuthorizationParameters.Nonce):
                    _nonce = (string)value;
                    break;
                case nameof(AuthorizationParameters.Prompts):
                    _prompts = (string[])value;
                    break;
                case nameof(AuthorizationParameters.RedirectUri):
                    _redirectUri = (string)value;
                    break;
                case nameof(AuthorizationParameters.ResponseMode):
                    _responseMode = (string)value;
                    break;
                case nameof(AuthorizationParameters.ResponseTypes):
                    _responseTypes = (string[])value;
                    break;
                case nameof(AuthorizationParameters.Scopes):
                    _scopes = (string[])value;
                    break;
                case nameof(AuthorizationParameters.State):
                    _state = (string)value;
                    break;
                default:
                    throw new NotSupportedException($"Unknown property {property}.");
            }
        }

        public void Throw(string error, string errorDescription)
        {
            var errorResponse = new AuthorizationErrorResponse(error, errorDescription, null, this._state, this._redirectUri, this._responseMode);

            throw new ErrorResponseException<AuthorizationErrorResponse>(errorResponse);
        }
    }
}
