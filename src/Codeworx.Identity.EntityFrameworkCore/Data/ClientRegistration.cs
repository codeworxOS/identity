using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.EntityFrameworkCore.Data
{
    public class ClientRegistration : IClientRegistration
    {
        public string ClientId { get; set; }

        public string ClientSecretHash { get; set; }

        public ClientType ClientType { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public RefreshTokenLifetime? RefreshTokenLifetime { get; set; }

        public TimeSpan? RefreshTokenExpiration { get; set; }

        public IReadOnlyList<Uri> ValidRedirectUrls { get; set; }

        public IUser User { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public bool AllowScim { get; set; }

        public string AccessTokenType { get; set; }

        public string AccessTokenTypeConfiguration { get; set; }
    }
}
