using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class UserPasswordHistory
    {
        public Guid UserId { get; set; }

        public User User { get; set; }

        [Key]
        [StringLength(512)]
        public string PasswordHash { get; set; }

        public DateTime ChangedAt { get; set; }
    }
}