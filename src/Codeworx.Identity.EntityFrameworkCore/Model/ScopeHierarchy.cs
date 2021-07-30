using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ScopeHierarchy
    {
        public Guid ParentId { get; set; }

        public Scope Parent { get; set; }

        public Guid ChildId { get; set; }

        public Scope Child { get; set; }
    }
}
