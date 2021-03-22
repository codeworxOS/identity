using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ScopeAssignment
    {
        public Guid ScopeId { get; set; }

        public Scope Scope { get; set; }

        public Guid ClientId { get; set; }

        public ClientConfiguration Client { get; set; }
    }
}