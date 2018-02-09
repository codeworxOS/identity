using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class Tenant
    {
        public Tenant()
        {
            this.Users = new HashSet<TenantUser>();
        }

        public Guid Id { get; set; }

        [StringLength(200)]
        [Required]
        public string Name { get; set; }

        public ICollection<TenantUser> Users { get; }
    }
}