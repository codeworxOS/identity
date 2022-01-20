using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Configuration
{
    public class IdentityOptions
    {
        public IdentityOptions()
        {
            AccountEndpoint = "/account";
            AuthenticationCookie = Constants.DefaultAuthenticationCookieName;
            AuthenticationScheme = Constants.DefaultAuthenticationScheme;
            CookieExpiration = TimeSpan.FromHours(1);
            InvitationValidity = TimeSpan.FromDays(60);
            OauthAuthorizationEndpoint = "/oauth20";
            OauthTokenEndpoint = OauthAuthorizationEndpoint + "/token";
            OpenIdAuthorizationEndpoint = "/openid10";
            OpenIdJsonWebKeyEndpoint = OpenIdAuthorizationEndpoint + "/certs";
            OpenIdTokenEndpoint = OpenIdAuthorizationEndpoint + "/token";
            OpenIdWellKnownPrefix = string.Empty;
            PasswordDescription = new Dictionary<string, string>()
            {
                { "en", Constants.DefaultPasswordDescriptionEn },
                { "de", Constants.DefaultPasswordDescriptionDe },
            };
            PasswordRegex = Constants.DefaultPasswordRegex;
            SelectTenantEndpoint = AccountEndpoint + "/tenant";
            Styles = new List<string> { Constants.Assets.Css.TrimStart('/') + "/style.css" };
            UserInfoEndpoint = "/userinfo";
            WindowsAuthenticationEnabled = false;
            CompanyName = "Identity";
            Favicon = Constants.DefaultFavicon;
            MaxFailedLogins = null;
        }

        public int? MaxFailedLogins { get; set; }

        public string AccountEndpoint { get; set; }

        public string AuthenticationCookie { get; set; }

        public string AuthenticationScheme { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public string Favicon { get; set; }

        public TimeSpan InvitationValidity { get; set; }

        public string OauthAuthorizationEndpoint { get; set; }

        public string OauthTokenEndpoint { get; set; }

        public string OpenIdAuthorizationEndpoint { get; set; }

        public string OpenIdJsonWebKeyEndpoint { get; set; }

        public string OpenIdTokenEndpoint { get; set; }

        public string OpenIdWellKnownPrefix { get; set; }

        public IDictionary<string, string> PasswordDescription { get; set; }

        public string PasswordRegex { get; set; }

        public string SelectTenantEndpoint { get; set; }

        public List<string> Styles { get; }

        public string UserInfoEndpoint { get; set; }

        public bool WindowsAuthenticationEnabled { get; set; }

        public string CompanyName { get; set; }

        public string SupportEmail { get; set; }

        public void CopyTo(IdentityOptions target)
        {
            target.AccountEndpoint = this.AccountEndpoint;
            target.AuthenticationCookie = this.AuthenticationCookie;
            target.AuthenticationScheme = this.AuthenticationScheme;
            target.CookieExpiration = this.CookieExpiration;
            target.Favicon = this.Favicon;
            target.InvitationValidity = this.InvitationValidity;
            target.OauthAuthorizationEndpoint = this.OauthAuthorizationEndpoint;
            target.OauthTokenEndpoint = this.OauthTokenEndpoint;
            target.OpenIdAuthorizationEndpoint = this.OpenIdAuthorizationEndpoint;
            target.OpenIdJsonWebKeyEndpoint = this.OpenIdJsonWebKeyEndpoint;
            target.OpenIdTokenEndpoint = this.OpenIdTokenEndpoint;
            target.OpenIdWellKnownPrefix = this.OpenIdWellKnownPrefix;
            target.PasswordDescription = this.PasswordDescription;
            target.PasswordRegex = this.PasswordRegex;
            target.SelectTenantEndpoint = this.SelectTenantEndpoint;
            target.CompanyName = this.CompanyName;
            target.SupportEmail = this.SupportEmail;

            target.Styles.Clear();

            foreach (var item in this.Styles)
            {
                target.Styles.Add(item);
            }

            target.UserInfoEndpoint = this.UserInfoEndpoint;
            target.WindowsAuthenticationEnabled = this.WindowsAuthenticationEnabled;
            target.MaxFailedLogins = this.MaxFailedLogins;
        }
    }
}