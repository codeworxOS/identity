using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class AvailableLicense
    {
        public Guid Id { get; set; }

        public Guid? TenantId { get; set; }

        public Tenant Tenant { get; set; }

        public Guid LicenseId { get; set; }

        public License License { get; set; }

        public int Quantity { get; set; }

        public DateTime ValidUntil { get; set; }
    }
}
