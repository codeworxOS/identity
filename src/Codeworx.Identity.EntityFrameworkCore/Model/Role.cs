using System;
using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class Role : RightHolder
    {
        public Role()
        {
            this.Members = new HashSet<RightHolder>();
        }

        public ICollection<RightHolder> Members { get; set; }

        public Tenant Tenant { get; set; }

        public Guid TenantId { get; set; }
    }
}