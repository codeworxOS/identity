using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Configuration
{
    public class IdentityOptions
    {
        public IdentityOptions()
        {
            OauthAuthorizationEndpoint = "/oauth20";
            OauthTokenEndpoint = OauthAuthorizationEndpoint + "/token";
            OpenIdAuthorizationEndpoint = "/openid10";
            OpenIdTokenEndpoint = OpenIdAuthorizationEndpoint + "/token";
            OpenIdJsonWebKeyEndpoint = OpenIdAuthorizationEndpoint + "/certs";
            OpenIdWellKnownPrefix = string.Empty;
            UserInfoEndpoint = "/userinfo";
            AccountEndpoint = "/account";
            SelectTenantEndpoint = AccountEndpoint + "/tenant";
            CookieExpiration = TimeSpan.FromHours(1);
            Styles = new HashSet<string>();
            AuthenticationScheme = Constants.DefaultAuthenticationScheme;
            AuthenticationCookie = Constants.DefaultAuthenticationCookieName;
            WindowsAuthenticationEnabled = false;
        }

        public string AccountEndpoint { get; set; }

        public string SelectTenantEndpoint { get; set; }

        public string AuthenticationCookie { get; set; }

        public string AuthenticationScheme { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public string OauthAuthorizationEndpoint { get; set; }

        public string OauthTokenEndpoint { get; set; }

        public string OpenIdAuthorizationEndpoint { get; set; }

        public string OpenIdTokenEndpoint { get; set; }

        public string OpenIdWellKnownPrefix { get; set; }

        public string OpenIdJsonWebKeyEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public HashSet<string> Styles { get; }

        public bool WindowsAuthenticationEnabled { get; set; }

        public void CopyTo(IdentityOptions target)
        {
            target.AuthenticationCookie = this.AuthenticationCookie;
            target.AuthenticationScheme = this.AuthenticationScheme;
            target.WindowsAuthenticationEnabled = this.WindowsAuthenticationEnabled;

            target.AccountEndpoint = this.AccountEndpoint;
            target.CookieExpiration = this.CookieExpiration;
            target.OauthAuthorizationEndpoint = this.OauthAuthorizationEndpoint;
            target.OauthTokenEndpoint = this.OauthTokenEndpoint;
            target.OpenIdAuthorizationEndpoint = this.OpenIdAuthorizationEndpoint;
            target.OpenIdTokenEndpoint = this.OpenIdTokenEndpoint;
            target.OpenIdJsonWebKeyEndpoint = this.OpenIdJsonWebKeyEndpoint;
            target.UserInfoEndpoint = this.UserInfoEndpoint;
            target.Styles.Clear();

            foreach (var item in this.Styles)
            {
                target.Styles.Add(item);
            }
        }
    }
}