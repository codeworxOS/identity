using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public byte[] ClientSecret { get; set; }

        public byte[] ClientSecretSalt { get; set; }

        public Guid Id { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        public string ClientId => this.Id.ToString("N");

        public byte[] ClientSecretHash => this.ClientSecret;

        public FlowType FlowTypes { get; set; }

        IReadOnlyList<ISupportedFlow> IClientRegistration.SupportedFlow => SupportedFlows.GetFlow(this.FlowTypes);

        IReadOnlyList<string> IClientRegistration.ValidRedirectUrls => new ReadOnlyCollection<string>(this.ValidRedirectUrls.Select(p => p.Url).ToList());

        public string DefaultRedirectUri { get; set; }

        Uri IClientRegistration.DefaultRedirectUri => new Uri(this.DefaultRedirectUri);

        public ICollection<ValidRedirectUrl> ValidRedirectUrls { get; }
    }
}