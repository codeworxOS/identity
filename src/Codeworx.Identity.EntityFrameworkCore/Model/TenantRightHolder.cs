using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class TenantRightHolder
    {
        public Guid RightHolderId { get; set; }

        public Tenant Tenant { get; set; }

        public Guid TenantId { get; set; }

        public RightHolder RightHolder { get; set; }
    }
}