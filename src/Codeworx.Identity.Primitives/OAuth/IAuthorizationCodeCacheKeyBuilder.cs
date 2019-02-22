namespace Codeworx.Identity.OAuth
{
    public interface IAuthorizationCodeCacheKeyBuilder
    {
        string Get(AuthorizationRequest request, string userIdentifier);
    }
}
