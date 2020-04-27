using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Codeworx.Identity.Model;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class Scope : IScope
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