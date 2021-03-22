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
            this.Children = new HashSet<ScopeHierarchy>();
        }

        public ICollection<ScopeClaim> Claims { get; }

        public ScopeHierarchy Parent { get; set; }

        public Guid Id { get; set; }

        [Required]
        [StringLength(50)]
        public string ScopeKey { get; set; }

        public ICollection<ScopeHierarchy> Children { get; }
    }
}