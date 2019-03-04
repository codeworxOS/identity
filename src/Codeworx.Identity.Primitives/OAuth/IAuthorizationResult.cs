namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationResult
    {
        AuthorizationErrorResponse Error { get; }

        AuthorizationCodeResponse Response { get; }
    }
}