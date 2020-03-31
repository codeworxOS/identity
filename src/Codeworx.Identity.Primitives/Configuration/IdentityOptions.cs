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
            AccountEndpoint = "/account";
            CookieExpiration = TimeSpan.FromHours(1);
            Styles = new HashSet<string>();
            AuthenticationScheme = Constants.DefaultAuthenticationScheme;
            AuthenticationCookie = Constants.DefaultAuthenticationCookieName;
            MissingTenantAuthenticationScheme = Constants.DefaultMissingTenantAuthenticationScheme;
            MissingTenantAuthenticationCookie = Constants.DefaultMissingTenantCookieName;
            WindowsAuthenticationEnabled = false;
        }

        public string AccountEndpoint { get; private set; }

        public string AuthenticationCookie { get; private set; }

        public string AuthenticationScheme { get; private set; }

        public TimeSpan CookieExpiration { get; private set; }

        public string MissingTenantAuthenticationCookie { get; private set; }

        public string MissingTenantAuthenticationScheme { get; private set; }

        public string OauthAuthorizationEndpoint { get; private set; }

        public string OauthTokenEndpoint { get; private set; }

        public string OpenIdAuthorizationEndpoint { get; private set; }

        public string OpenIdTokenEndpoint { get; private set; }

        public string OpenIdJsonWebKeyEndpoint { get; private set; }

        public HashSet<string> Styles { get; }

        public bool WindowsAuthenticationEnabled { get; private set; }

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
            target.Styles.Clear();

            foreach (var item in this.Styles)
            {
                target.Styles.Add(item);
            }
        }
    }
}