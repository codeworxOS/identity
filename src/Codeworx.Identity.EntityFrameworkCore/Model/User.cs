﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class User : RightHolder
    {
        public User()
        {
            this.Clients = new HashSet<ClientConfiguration>();
            this.IdentityCaches = new HashSet<IdentityCache>();
            this.Invitations = new HashSet<UserInvitation>();
            this.PasswordHistory = new HashSet<UserPasswordHistory>();
        }

        public Tenant DefaultTenant { get; set; }

        public Guid? DefaultTenantId { get; set; }

        public bool IsDisabled { get; set; }

        public bool ConfirmationPending { get; set; }

        [StringLength(512)]
        public string ConfirmationCode { get; set; }

        public bool ForceChangePassword { get; set; }

        public DateTime Created { get; set; }

        public DateTime PasswordChanged { get; set; }

        public DateTime? LastFailedLoginAttempt { get; set; }

        public int FailedLoginCount { get; set; }

        [StringLength(512)]
        public string PasswordHash { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ICollection<ClientConfiguration> Clients { get; }

        public ICollection<IdentityCache> IdentityCaches { get; }

        public ICollection<UserInvitation> Invitations { get; set; }

        public ICollection<UserPasswordHistory> PasswordHistory { get; set; }
    }
}