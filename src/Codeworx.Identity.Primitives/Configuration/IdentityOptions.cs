using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Configuration
{
    public class IdentityOptions
    {
        public IdentityOptions()
        {
            OauthEndpoint = "/oauth20";
            AccountEndpoint = "/account";
            CookieExpiration = TimeSpan.FromHours(1);
            AuthenticationCookie = "identity";
            Styles = new HashSet<string>();
        }

        public string AccountEndpoint { get; set; }

        public string AuthenticationCookie { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public string OauthEndpoint { get; set; }

        public HashSet<string> Styles { get; }

        public void CopyTo(IdentityOptions target)
        {
            target.AuthenticationCookie = this.AuthenticationCookie;
            target.AccountEndpoint = this.AccountEndpoint;
            target.CookieExpiration = this.CookieExpiration;
            target.OauthEndpoint = this.OauthEndpoint;
            target.Styles.Clear();
            foreach (var item in this.Styles)
            {
                target.Styles.Add(item);
            }
        }
    }
}