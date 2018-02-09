using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ScopeClaim
    {
        public ClaimType ClaimType { get; set; }

        [Column(Order = 2)]
        [Key]
        public Guid ClaimTypeId { get; set; }

        public Scope Scope { get; set; }

        [Column(Order = 1)]
        [Key]
        public Guid ScopeId { get; set; }
    }
}