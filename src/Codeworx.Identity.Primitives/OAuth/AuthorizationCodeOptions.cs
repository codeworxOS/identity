using System;

namespace Codeworx.Identity.OAuth
{
    public class AuthorizationCodeOptions
    {
        public double ExpirationInSeconds { get; set; } = TimeSpan.FromMinutes(10).TotalSeconds;

        public int Length { get; set; } = 64;

        public void CopyTo(AuthorizationCodeOptions target)
        {
            target.Length = this.Length;
            target.ExpirationInSeconds = this.ExpirationInSeconds;
        }
    }
}