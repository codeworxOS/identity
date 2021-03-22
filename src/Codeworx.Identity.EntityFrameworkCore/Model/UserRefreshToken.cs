using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class UserRefreshToken
    {
        public Guid UserId { get; set; }

        public User User { get; set; }

        [StringLength(4000)]
        [Required]
        [Key]
        public string Token { get; set; }

        [Required]
        public string IdentityData { get; set; }

        public Guid ClientId { get; set; }

        public ClientConfiguration Client { get; set; }

        public DateTime ValidUntil { get; set; }

        public bool IsDisabled { get; set; }
    }
}
