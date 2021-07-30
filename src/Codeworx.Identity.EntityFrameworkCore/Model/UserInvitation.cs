using System;
using System.ComponentModel.DataAnnotations;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class UserInvitation
    {
        [StringLength(4000)]
        [Key]
        [Required]
        public string InvitationCode { get; set; }

        public Guid UserId { get; set; }

        public User User { get; set; }

        public bool CanChangeLogin { get; set; }

        [StringLength(2000)]
        public string RedirectUri { get; set; }

        public DateTime ValidUntil { get; set; }

        public bool IsDisabled { get; set; }
    }
}
