namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationResult
    {
        AuthorizationResponse Response { get; }
    }
}