namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public enum RefreshTokenLifetime
    {
        UseOnce = 0x00,
        SlidingExpiration = 0x01,
        RecreatAfterHalfLifetime = 0x02
    }
}