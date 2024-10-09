using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class ClientConfigurationInfoData
    {
        public Guid Id { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public RefreshTokenLifetime? RefreshTokenLifetime { get; set; }

        public TimeSpan? RefreshTokenExpiration { get; set; }

        public string AccessTokenType { get; set; }

        public bool AllowScim { get; set; }

        public Dictionary<string, object> AccessTokenTypeConfiguration { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ClientType ClientType { get; set; }

        public UserInfoData User { get; set; }

        public List<ScopeInfoData> Scopes { get; set; }

        public List<ValidRedirectUrlInfoData> ValidRedirectUrls { get; set; }

        public string ClientSecret { get; set; }

        public bool HasClientSecret { get; set; }
    }
}
