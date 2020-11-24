using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
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

        [StringLength(512)]
        public string ClientSecretHash { get; set; }

        public Guid Id { get; set; }

        public TimeSpan TokenExpiration { get; set; }

        IReadOnlyList<Uri> IClientRegistration.ValidRedirectUrls => this.ValidRedirectUrls.Select(p => new Uri(p.Url)).ToImmutableList();

        public ICollection<ValidRedirectUrl> ValidRedirectUrls { get; }

        public ClientType ClientType { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        IUser IClientRegistration.User => this.User;
    }
}