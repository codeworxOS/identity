using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;
using Codeworx.Identity.Token;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class ClientConfigurationInsertData
    {
        public TimeSpan TokenExpiration { get; set; }

        public string AccessTokenType { get; set; }

        public bool AllowScim { get; set; }

        public Dictionary<string, object> AccessTokenTypeConfiguration { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ClientType ClientType { get; set; }

        public RefreshTokenLifetime? RefreshTokenLifetime { get; set; }

        public TimeSpan? RefreshTokenExpiration { get; set; }

        public UserInfoData User { get; set; }
    }
}
