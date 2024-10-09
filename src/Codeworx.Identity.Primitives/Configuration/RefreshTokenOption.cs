using System;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.Configuration
{
    public class RefreshTokenOption
    {
        public RefreshTokenOption(RefreshTokenOption source)
        {
            Lifetime = source.Lifetime;
            Expiration = source.Expiration;
        }

        public RefreshTokenOption()
        {
            Lifetime = RefreshTokenLifetime.RecreateAfterHalfLifetime;
            Expiration = TimeSpan.FromDays(180);
        }

        public RefreshTokenLifetime Lifetime { get; set; }

        public TimeSpan Expiration { get; set; }
    }
}
