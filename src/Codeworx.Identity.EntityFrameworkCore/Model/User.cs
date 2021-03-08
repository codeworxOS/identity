using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class User : RightHolder
    {
        public User()
        {
            this.Tenants = new HashSet<TenantUser>();
            this.Clients = new HashSet<ClientConfiguration>();
            this.RefreshTokens = new HashSet<UserRefreshToken>();
            this.Invitations = new HashSet<UserInvitation>();
        }

        public Tenant DefaultTenant { get; set; }

        public Guid? DefaultTenantId { get; set; }

        public bool IsDisabled { get; set; }

        public bool ForceChangePassword { get; set; }

        public DateTime Created { get; set; }

        public DateTime PasswordChanged { get; set; }

        public DateTime? LastFailedLoginAttempt { get; set; }

        public int FailedLoginCount { get; set; }

        [StringLength(512)]
        public string PasswordHash { get; set; }

        public ICollection<TenantUser> Tenants { get; }

        public ICollection<ClientConfiguration> Clients { get; }

        public ICollection<UserRefreshToken> RefreshTokens { get; }

        public ICollection<UserInvitation> Invitations { get; set; }
    }
}