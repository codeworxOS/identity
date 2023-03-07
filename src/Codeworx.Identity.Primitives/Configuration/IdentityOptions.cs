using System;
using System.Collections.Generic;

namespace Codeworx.Identity.Configuration
{
    public class IdentityOptions
    {
        public IdentityOptions()
        {
            InvitationValidity = TimeSpan.FromDays(60);
            StateLookupCacheExpiration = TimeSpan.FromMinutes(5);

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
            MaxLength = new MaxLengthOption
            {
                Login = 500,
                Password = 100,
            };
            Signing = new SigningOptions();
            Styles = new List<string> { Constants.Assets.Css.TrimStart('/') + "/font-awesome.min.css?v=6.2.1", Constants.Assets.Css.TrimStart('/') + "/style.css?v=1.0.0" };
            Scripts = new List<string> { Constants.Assets.Js.TrimStart('/') + "/main.js?v=1.0.0" };
            Preloads = new Dictionary<string, PreloadOption>
            {
                {
                    "fa-regular",
                    new PreloadOption
                    {
                        Enable = true,
                        Type = PreloadType.Font,
                        Version = "6.2.1",
                        Files =
                        {
                            { Constants.Assets.WebFonts.TrimStart('/') + "/fa-regular-400.woff2", "font/woff2" },
                        },
                    }
                },
                {
                    "fa-brands",
                    new PreloadOption
                    {
                        Enable = true,
                        Type = PreloadType.Font,
                        Version = "6.2.1",
                        Files =
                        {
                            { Constants.Assets.WebFonts.TrimStart('/') + "/fa-brands-400.woff2", "font/woff2" },
                        },
                    }
                },
                {
                    "fa-solid",
                    new PreloadOption
                    {
                        Enable = true,
                        Type = PreloadType.Font,
                        Version = "6.2.1",
                        Files =
                        {
                            { Constants.Assets.WebFonts.TrimStart('/') + "/fa-solid-900.woff2", "font/woff2" },
                        },
                    }
                },
            };
            CompanyName = "Identity";
            Favicon = Constants.DefaultFavicon;
            MaxFailedLogins = null;
            PasswordHistoryLength = 0;
            EnableAccountConfirmation = false;
            FormsPersistenceMode = FormsPersistenceMode.SessionWithPersistOption;
        }

        public int? MaxFailedLogins { get; set; }

        public TimeSpan StateLookupCacheExpiration { get; set; }

        public string Favicon { get; set; }

        public TimeSpan InvitationValidity { get; set; }

        public RegexPolicyOption Password { get; set; }

        public RegexPolicyOption Login { get; set; }

        public MaxLengthOption MaxLength { get; set; }

        public SigningOptions Signing { get; set; }

        public List<string> Styles { get; }

        public List<string> Scripts { get; }

        public Dictionary<string, PreloadOption> Preloads { get; }

        public string CompanyName { get; set; }

        public string SupportEmail { get; set; }

        public bool EnableAccountConfirmation { get; set; }

        public FormsPersistenceMode FormsPersistenceMode { get; set; }

        public int PasswordHistoryLength { get; set; }

        public void CopyTo(IdentityOptions target)
        {
            target.FormsPersistenceMode = this.FormsPersistenceMode;
            target.Favicon = this.Favicon;
            target.InvitationValidity = this.InvitationValidity;
            target.Password = this.Password != null ? new RegexPolicyOption(this.Password) : null;
            target.Login = this.Login != null ? new RegexPolicyOption(this.Login) : null;
            target.MaxLength = this.MaxLength != null ? new MaxLengthOption(this.MaxLength) : null;
            target.CompanyName = this.CompanyName;
            target.SupportEmail = this.SupportEmail;
            target.Signing = new SigningOptions(this.Signing);

            target.Styles.Clear();

            foreach (var item in this.Styles)
            {
                target.Styles.Add(item);
            }

            target.Scripts.Clear();

            foreach (var item in this.Scripts)
            {
                target.Scripts.Add(item);
            }

            target.Preloads.Clear();

            foreach (var item in this.Preloads)
            {
                target.Preloads.Add(item.Key, item.Value.Clone());
            }

            target.MaxFailedLogins = this.MaxFailedLogins;
            target.PasswordHistoryLength = this.PasswordHistoryLength;
        }
    }
}