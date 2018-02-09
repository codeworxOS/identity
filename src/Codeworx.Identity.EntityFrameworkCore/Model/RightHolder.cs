using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class RightHolder
    {
        public RightHolder()
        {
            this.MemberOf = new HashSet<UserRole>();
        }

        public Guid Id { get; set; }

        public ICollection<UserRole> MemberOf { get; }

        [StringLength(500)]
        [Required]
        public string Name { get; set; }
    }
}