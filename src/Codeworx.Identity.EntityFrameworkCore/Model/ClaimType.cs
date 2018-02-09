using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClaimType
    {
        public Guid Id { get; set; }

        public ClaimTarget Target { get; set; }

        [StringLength(50)]
        [Required]
        public string TypeKey { get; set; }
    }
}