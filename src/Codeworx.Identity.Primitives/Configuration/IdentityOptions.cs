using System;

namespace Codeworx.Identity.Configuration
{
    public class IdentityOptions
    {
        public IdentityOptions()
        {
            OauthEndpoint = "/oauth20";
            CookieExpiration = TimeSpan.FromHours(1);
            AuthenticationCookie = "identity";
        }

        public string AuthenticationCookie { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public string OauthEndpoint { get; set; }
    }
}