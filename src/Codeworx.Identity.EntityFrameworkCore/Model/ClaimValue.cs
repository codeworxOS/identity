using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClaimValue
    {
        public Guid ClaimTypeId { get; set; }

        public Guid Id { get; set; }

        public Guid? RightHolderId { get; set; }

        public Guid? TenantId { get; set; }

        [StringLength(500)]
        public string Value { get; set; }
    }
}