using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClaimType
    {
        public ClaimType()
        {
            this.ScopeClaims = new HashSet<ScopeClaim>();
        }

        public Guid Id { get; set; }

        public ClaimTarget Target { get; set; }

        [StringLength(50)]
        [Required]
        public string TypeKey { get; set; }

        public ICollection<ScopeClaim> ScopeClaims { get; }
    }
}