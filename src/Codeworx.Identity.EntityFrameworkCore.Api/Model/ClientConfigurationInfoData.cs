using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class ClientConfigurationInfoData
    {
        public Guid Id { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public string AccessTokenType { get; set; }

        public string AccessTokenTypeConfiguration { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ClientType ClientType { get; set; }

        public UserInfoData User { get; set; }

        public List<ScopeInfoData> Scopes { get; set; }

        public List<string> ValidRedirectUrls { get; set; }

        public string ClientSecret { get; set; }

        public bool HasClientSecret { get; set; }
    }
}
