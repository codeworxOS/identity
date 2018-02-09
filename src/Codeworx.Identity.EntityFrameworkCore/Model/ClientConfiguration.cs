using System;
using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClientConfiguration
    {
        public ClientConfiguration()
        {
            this.AllowedScopes = new HashSet<ClientScope>();
        }

        public ICollection<ClientScope> AllowedScopes { get; }

        public byte[] ClientSecret { get; set; }

        public byte[] ClientSecretSalt { get; set; }

        public EndpointProtocol EnabledProtocols { get; set; }

        public Guid Id { get; set; }

        public bool IsConfidential { get; set; }

        public TimeSpan RefreshTokenExpiration { get; set; }

        public RefreshTokenLifetime RefreshTokenLifetime { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public User User { get; set; }

        public Guid? UserId { get; set; }
    }
}