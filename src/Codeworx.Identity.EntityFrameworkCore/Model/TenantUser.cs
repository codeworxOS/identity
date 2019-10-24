using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class TenantUser
    {
        public Guid RightHolderId { get; set; }

        public Tenant Tenant { get; set; }

        public Guid TenantId { get; set; }

        public User User { get; set; }
    }
}