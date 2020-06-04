namespace Codeworx.Identity.OAuth.Authorization
{
    public interface IAuthorizationResponseBuilder
    {
        AuthorizationSuccessResponse Response { get; }

        IAuthorizationResponseBuilder WithState(string state);

        IAuthorizationResponseBuilder WithRedirectUri(string redirectUri);

        void RaiseError(string error);
    }
}
