namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public enum CacheType
    {
        AuthorizationCode = 0x00,
        AccessToken = 0x01,
        Lookup = 0x02,
        ExternalTokenData = 0x03,
    }
}