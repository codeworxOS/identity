using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class License
    {
        public License()
        {
            this.Clients = new HashSet<ClientLicense>();
        }

        public Guid Id { get; set; }

        [StringLength(200)]
        [Required]
        public string Name { get; set; }

        public ICollection<ClientLicense> Clients { get; }
    }
}
