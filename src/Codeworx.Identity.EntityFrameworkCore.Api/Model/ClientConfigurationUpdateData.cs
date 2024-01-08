using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class ClientConfigurationUpdateData
    {
        public Guid Id { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public string AccessTokenType { get; set; }

        public Dictionary<string, object> AccessTokenTypeConfiguration { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ClientType ClientType { get; set; }

        public UserInfoData User { get; set; }
    }
}
