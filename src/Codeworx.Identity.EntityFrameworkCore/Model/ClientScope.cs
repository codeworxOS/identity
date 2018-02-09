using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClientScope
    {
        public ClientConfiguration Client { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ClientId { get; set; }

        public Scope Scope { get; set; }

        [Key]
        [Column(Order = 1)]
        public Guid ScopeId { get; set; }
    }
}