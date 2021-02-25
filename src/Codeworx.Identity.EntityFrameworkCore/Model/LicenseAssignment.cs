using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class LicenseAssignment
    {
        public Guid UserId { get; set; }

        public User User { get; set; }

        public Guid LicenseId { get; set; }

        public License License { get; set; }
    }
}
