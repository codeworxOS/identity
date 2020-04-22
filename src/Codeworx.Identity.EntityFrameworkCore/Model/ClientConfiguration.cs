using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClientConfiguration : IClientRegistration
    {
        public ClientConfiguration()
        {
            this.AllowedScopes = new HashSet<ClientScope>();
            this.ValidRedirectUrls = new HashSet<ValidRedirectUrl>();
        }

        public ICollection<ClientScope> AllowedScopes { get; }

        public string ClientId => this.Id.ToString("N");

        public byte[] ClientSecret { get; set; }

        public byte[] ClientSecretHash => this.ClientSecret;

        public byte[] ClientSecretSalt { get; set; }

        public string DefaultRedirectUri { get; set; }

        public FlowType FlowTypes { get; set; }

        public Guid Id { get; set; }

        IReadOnlyList<ISupportedFlow> IClientRegistration.SupportedFlow => SupportedFlows.GetFlow(this.FlowTypes);

        public TimeSpan TokenExpiration { get; set; }

        IReadOnlyList<Uri> IClientRegistration.ValidRedirectUrls => this.ValidRedirectUrls.Select(p => new Uri(p.Url)).ToImmutableList();

        public ICollection<ValidRedirectUrl> ValidRedirectUrls { get; }
    }
}