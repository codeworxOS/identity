using System;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.Configuration.Infrastructure
{
    public class ClientConfig
    {
        public ClientConfig()
        {
            TokenExpiration = TimeSpan.FromHours(1);
            Type = ClientType.WebBackend;
        }

        public string[] RedirectUris { get; set; }

        public string[] AllowedScopes { get; set; }

        public string Secret { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public ClientType Type { get; set; }

        public string User { get; set; }
    }
}