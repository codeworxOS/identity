using System;
using System.Collections.Generic;
using Codeworx.Identity.Login;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Data
{
    public class ClientRegistration : IClientRegistration
    {
        public string ClientId { get; set; }

        public string ClientSecretHash { get; set; }

        public ClientType ClientType { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public IReadOnlyList<Uri> ValidRedirectUrls { get; set; }

        public IUser User { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }
    }
}
