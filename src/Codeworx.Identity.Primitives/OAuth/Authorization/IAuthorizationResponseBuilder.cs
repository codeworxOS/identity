using System;

namespace Codeworx.Identity.OAuth.Authorization
{
    public interface IAuthorizationResponseBuilder
    {
        AuthorizationSuccessResponse Response { get; }

        IAuthorizationResponseBuilder WithState(string state);

        IAuthorizationResponseBuilder WithRedirectUri(string redirectUri);

        void RaiseError(string error);

        IAuthorizationResponseBuilder WithCode(string authorizationCode);

        IAuthorizationResponseBuilder WithAccessToken(string accessToken, TimeSpan tokenExpiration);

        IAuthorizationResponseBuilder WithIdToken(string identityToken);
    }
}
