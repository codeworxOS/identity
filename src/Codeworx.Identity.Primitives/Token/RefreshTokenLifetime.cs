namespace Codeworx.Identity.Token
{
    public enum RefreshTokenLifetime
    {
        UseOnce = 0x00,
        SlidingExpiration = 0x01,
        RecreateAfterHalfLifetime = 0x02,
    }
}