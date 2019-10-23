using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class UserRole
    {
        public Guid Id { get; set; }

        public Role Role { get; set; }

        public Guid RoleId { get; set; }

        public User User { get; set; }

        public Guid UserId { get; set; }
    }
}