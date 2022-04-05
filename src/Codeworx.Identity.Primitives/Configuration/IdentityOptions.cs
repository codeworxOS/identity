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
            StateLookupCacheExpiration = TimeSpan.FromMinutes(5);
            OauthAuthorizationEndpoint = "/oauth20";
            OauthTokenEndpoint = OauthAuthorizationEndpoint + "/token";
            OpenIdAuthorizationEndpoint = "/openid10";
            OpenIdJsonWebKeyEndpoint = OpenIdAuthorizationEndpoint + "/certs";
            OpenIdTokenEndpoint = OpenIdAuthorizationEndpoint + "/token";
            OpenIdWellKnownPrefix = string.Empty;
            Password = new RegexPolicyOption
            {
                Regex = Constants.DefaultPasswordRegex,
                Description =
                {
                    { "en", Constants.DefaultPasswordDescriptionEn },
                    { "de", Constants.DefaultPasswordDescriptionDe },
                },
            };
            Login = new RegexPolicyOption
            {
                Regex = Constants.DefaultLoginRegex,
                Description =
                {
                    { "en", Constants.DefaultLoginDescriptionEn },
                    { "de", Constants.DefaultLoginDescriptionDe },
                },
            };
            SelectTenantEndpoint = AccountEndpoint + "/tenant";
            Styles = new List<string> { Constants.Assets.Css.TrimStart('/') + "/style.css" };
            UserInfoEndpoint = "/userinfo";
            WindowsAuthenticationEnabled = false;
            CompanyName = "Identity";
            Favicon = Constants.DefaultFavicon;
            MaxFailedLogins = null;
            PasswordHistoryLength = 0;
            EnableAccountConfirmation = false;
        }

        public int? MaxFailedLogins { get; set; }

        public string AccountEndpoint { get; set; }

        public string AuthenticationCookie { get; set; }

        public string AuthenticationScheme { get; set; }

        public TimeSpan CookieExpiration { get; set; }

        public TimeSpan StateLookupCacheExpiration { get; set; }

        public string Favicon { get; set; }

        public TimeSpan InvitationValidity { get; set; }

        public string OauthAuthorizationEndpoint { get; set; }

        public string OauthTokenEndpoint { get; set; }

        public string OpenIdAuthorizationEndpoint { get; set; }

        public string OpenIdJsonWebKeyEndpoint { get; set; }

        public string OpenIdTokenEndpoint { get; set; }

        public string OpenIdWellKnownPrefix { get; set; }

        public RegexPolicyOption Password { get; set; }

        public RegexPolicyOption Login { get; set; }

        public string SelectTenantEndpoint { get; set; }

        public List<string> Styles { get; }

        public string UserInfoEndpoint { get; set; }

        public bool WindowsAuthenticationEnabled { get; set; }

        public string CompanyName { get; set; }

        public string SupportEmail { get; set; }

        public bool EnableAccountConfirmation { get; set; }

        public int PasswordHistoryLength { get; set; }

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
            target.Password = this.Password != null ? new RegexPolicyOption(this.Password) : null;
            target.Login = this.Login != null ? new RegexPolicyOption(this.Login) : null;
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
            target.PasswordHistoryLength = this.PasswordHistoryLength;
        }
    }
}