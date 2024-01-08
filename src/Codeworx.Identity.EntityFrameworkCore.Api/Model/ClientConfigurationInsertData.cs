using System;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class ClientConfigurationInsertData
    {
        public TimeSpan TokenExpiration { get; set; }

        public string AccessTokenType { get; set; }

        public string AccessTokenTypeConfiguration { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ClientType ClientType { get; set; }

        public UserInfoData User { get; set; }
    }
}
