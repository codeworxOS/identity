using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public abstract class RightHolder
    {
        public RightHolder()
        {
            this.Tenants = new HashSet<TenantRightHolder>();
            this.MemberOf = new HashSet<RightHolderGroup>();
            this.Providers = new HashSet<AuthenticationProviderRightHolder>();
        }

        public Guid Id { get; set; }

        public ICollection<RightHolderGroup> MemberOf { get; }

        public ICollection<AuthenticationProviderRightHolder> Providers { get; }

        [StringLength(500)]
        [Required]
        public string Name { get; set; }

        public ICollection<TenantRightHolder> Tenants { get; }
    }
}