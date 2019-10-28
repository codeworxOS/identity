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
            Styles = new HashSet<string>();
            AuthenticationScheme = Constants.DefaultAuthenticationScheme;
            AuthenticationCookie = Constants.DefaultAuthenticationCookieName;
            MissingTenantAuthenticationScheme = Constants.DefaultMissingTenantAuthenticationScheme;
            MissingTenantAuthenticationCookie = Constants.DefaultMissingTenantCookieName;
            WindowsAuthenticationEnabled = false;
        }

        public string AccountEndpoint { get; set; }

        public string AuthenticationCookie { get; set; }

        public string MissingTenantAuthenticationCookie { get; set; }

        public bool WindowsAuthenticationEnabled { get; }

        public TimeSpan CookieExpiration { get; set; }

        public string OauthEndpoint { get; set; }

        public HashSet<string> Styles { get; }

        public string AuthenticationScheme { get; set; }

        public string MissingTenantAuthenticationScheme { get; set; }

        public void CopyTo(IdentityOptions target)
        {
            target.AuthenticationCookie = this.AuthenticationCookie;
            target.AuthenticationScheme = this.AuthenticationScheme;
            target.MissingTenantAuthenticationCookie = this.MissingTenantAuthenticationCookie;
            target.MissingTenantAuthenticationScheme = this.MissingTenantAuthenticationScheme;

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