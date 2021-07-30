using System;

namespace Codeworx.Identity.EntityFrameworkCore.Model
{
    public class ValidRedirectUrl
    {
        public Guid Id { get; set; }

        public Guid ClientId { get; set; }

        public ClientConfiguration Client { get; set; }

        public string Url { get; set; }
    }
}