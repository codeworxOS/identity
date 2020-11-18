using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class User : RightHolder, IUser
    {
        public User()
        {
            this.Tenants = new HashSet<TenantUser>();
            this.Providers = new HashSet<AuthenticationProviderUser>();
            this.Clients = new HashSet<ClientConfiguration>();
        }

        public Tenant DefaultTenant { get; set; }

        public Guid? DefaultTenantId { get; set; }

        public string DefaultTenantKey => DefaultTenantId?.ToString("N");

        public string Identity => Id.ToString("N");

        public bool IsDisabled { get; set; }

        [StringLength(512)]
        public string PasswordHash { get; set; }

        public ICollection<TenantUser> Tenants { get; }

        public ICollection<AuthenticationProviderUser> Providers { get; }

        public ICollection<ClientConfiguration> Clients { get; }
    }
}