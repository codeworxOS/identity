namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public enum CacheType
    {
        AuthorizationCode = 0x00,
        RefreshToken = 0x01,
        AccessToken = 0x02,
    }
}