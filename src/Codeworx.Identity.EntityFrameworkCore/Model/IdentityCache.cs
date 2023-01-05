using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class IdentityCache
    {
        [Key]
        [StringLength(2000)]
        [Required]
        public string Key { get; set; }

        [Required]
        public string Value { get; set; }

        public Guid? UserId { get; set; }

        public User User { get; set; }

        public DateTime ValidUntil { get; set; }

        public CacheType CacheType { get; set; }

        public bool Disabled { get; set; }
    }
}
