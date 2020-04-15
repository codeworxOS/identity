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
            UserInfoEndpoint = "/userinfo";
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

        public string AuthenticationScheme { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public string MissingTenantAuthenticationCookie { get; set; }

        public string MissingTenantAuthenticationScheme { get; set; }

        public string OauthAuthorizationEndpoint { get; set; }

        public string OauthTokenEndpoint { get; set; }

        public string OpenIdAuthorizationEndpoint { get; set; }

        public string OpenIdTokenEndpoint { get; set; }

        public string OpenIdJsonWebKeyEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }

        public HashSet<string> Styles { get; }

        public bool WindowsAuthenticationEnabled { get; set; }

        public void CopyTo(IdentityOptions target)
        {
            target.AuthenticationCookie = this.AuthenticationCookie;
            target.AuthenticationScheme = this.AuthenticationScheme;
            target.MissingTenantAuthenticationCookie = this.MissingTenantAuthenticationCookie;
            target.MissingTenantAuthenticationScheme = this.MissingTenantAuthenticationScheme;
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