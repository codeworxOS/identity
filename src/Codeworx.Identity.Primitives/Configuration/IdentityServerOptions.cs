using System;

namespace Codeworx.Identity.Configuration
{
    public class IdentityServerOptions
    {
        public IdentityServerOptions()
        {
            AuthenticationCookie = Constants.DefaultAuthenticationCookieName;
            AuthenticationScheme = Constants.DefaultAuthenticationScheme;
            MfaAuthenticationCookie = Constants.DefaultMfaAuthenticationCookieName;
            MfaAuthenticationScheme = Constants.DefaultMfaAuthenticationScheme;
            CookieExpiration = TimeSpan.FromHours(1);
            UserInfoEndpoint = "/userinfo";
            AccountEndpoint = "/account";
            SelectTenantEndpoint = AccountEndpoint + "/tenant";
            OauthAuthorizationEndpoint = "/oauth20";
            OauthTokenEndpoint = OauthAuthorizationEndpoint + "/token";
            OauthInstrospectionEndpoint = OauthAuthorizationEndpoint + "/introspect";
            OpenIdAuthorizationEndpoint = "/openid10";
            OpenIdJsonWebKeyEndpoint = OpenIdAuthorizationEndpoint + "/certs";
            OpenIdTokenEndpoint = OpenIdAuthorizationEndpoint + "/token";
            OpenIdWellKnownPrefix = string.Empty;
        }

        public string AccountEndpoint { get; set; }

        public string AuthenticationCookie { get; set; }

        public string AuthenticationScheme { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public string MfaAuthenticationCookie { get; set; }

        public string MfaAuthenticationScheme { get; set; }

        public string OauthAuthorizationEndpoint { get; set; }

        public string OauthInstrospectionEndpoint { get; set; }

        public string OauthTokenEndpoint { get; set; }

        public string OpenIdAuthorizationEndpoint { get; set; }

        public string OpenIdJsonWebKeyEndpoint { get; set; }

        public string OpenIdTokenEndpoint { get; set; }

        public string OpenIdWellKnownPrefix { get; set; }

        public string SelectTenantEndpoint { get; set; }

        public string UserInfoEndpoint { get; set; }
    }
}
