using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClaimValue
    {
        public Guid ClaimTypeId { get; set; }

        public ClaimType ClaimType { get; set; }

        public Guid Id { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public Tenant Tenant { get; set; }

        public Guid? TenantId { get; set; }

        [StringLength(500)]
        public string Value { get; set; }
    }
}