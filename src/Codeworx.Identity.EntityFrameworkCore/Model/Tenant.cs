using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.Login;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class Tenant
    {
        public Tenant()
        {
            this.RightHolders = new HashSet<TenantRightHolder>();
        }

        public Guid Id { get; set; }

        [StringLength(200)]
        [Required]
        public string Name { get; set; }

        public AuthenticationMode AuthenticationMode { get; set; }

        public ICollection<TenantRightHolder> RightHolders { get; }
    }
}