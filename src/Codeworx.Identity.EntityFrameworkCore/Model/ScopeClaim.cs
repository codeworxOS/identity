using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ScopeClaim
    {
        public Guid ScopeId { get; set; }

        public Guid ClaimTypeId { get; set; }

        public Scope Scope { get; set; }

        public ClaimType ClaimType { get; set; }
    }
}
