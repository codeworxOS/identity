using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ClientLicense
    {
        public Guid ClientId { get; set; }

        public Guid LicenseId { get; set; }

        public ClientConfiguration Client { get; set; }

        public License License { get; set; }
    }
}