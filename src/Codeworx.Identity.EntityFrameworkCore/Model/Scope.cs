using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class Scope
    {
        public Scope()
        {
            this.Claims = new HashSet<ScopeClaim>();
        }

        public ICollection<ScopeClaim> Claims { get; }

        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ScopeKey { get; set; }
    }
}