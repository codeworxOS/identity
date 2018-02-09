using System;
using System.Collections.Generic;
using System.Text;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClaimAssignment
    {
        public Guid ClaimTypeId { get; set; }

        public Guid Id { get; set; }

        public Guid? RightHolderId { get; set; }

        public Guid? TenantId { get; set; }
    }
}