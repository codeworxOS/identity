using System;
using System.Collections.Generic;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class User : RightHolder, IUser
    {
        public User()
        {
            this.Tenants = new HashSet<TenantUser>();
            this.OAuthClientRegistrations = new HashSet<IOAuthClientRegistration>();
        }

        public Tenant DefaultTenant { get; set; }

        public Guid? DefaultTenantId { get; set; }

        public string DefaultTenantKey => DefaultTenantId?.ToString("N");

        public string Identity => Id.ToString("N");

        public ICollection<IOAuthClientRegistration> OAuthClientRegistrations { get; set; }

        public bool IsDisabled { get; set; }

        public byte[] PasswordHash { get; set; }

        public byte[] PasswordSalt { get; set; }

        public ICollection<TenantUser> Tenants { get; }
    }
}