using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public abstract class RightHolder
    {
        public RightHolder()
        {
            this.MemberOf = new HashSet<RightHolderGroup>();
        }

        public Guid Id { get; set; }

        public ICollection<RightHolderGroup> MemberOf { get; }

        [StringLength(500)]
        [Required]
        public string Name { get; set; }
    }
}