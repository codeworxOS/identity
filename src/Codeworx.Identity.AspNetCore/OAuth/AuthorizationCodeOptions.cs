using System;

namespace Codeworx.Identity.AspNetCore.OAuth
{
    public class AuthorizationCodeOptions
    {
        public int Length { get; set; } = 10;

        public double ExpirationInSeconds { get; set; } = TimeSpan.FromMinutes(10).TotalSeconds;
    }
}
