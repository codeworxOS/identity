using System;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.Configuration.Infrastructure
{
    public class ClientConfig
    {
        public ClientConfig()
        {
            TokenExpiration = TimeSpan.FromHours(1);
            Type = ClientType.WebBackend;
            AllowScim = false;
        }

        public string[] RedirectUris { get; set; }

        public string[] AllowedScopes { get; set; }

        public string Secret { get; set; }

        public bool AllowScim { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public ClientType Type { get; set; }

        public string User { get; set; }

        public string AccessTokenType { get; set; }

        public string AccessTokenTypeConfiguration { get; set; }

        public RefreshTokenLifetime? RefreshTokenLifetime { get; set; }

        public TimeSpan? RefreshTokenExpiration { get; set; }
    }
}