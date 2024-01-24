using System;

namespace Codeworx.Identity.EntityFrameworkCore.Api.Model
{
    public class ApiTokenRequest
    {
        public string Scopes { get; set; }

        public Guid ClientId { get; set; }

        public string ClientSecret { get; set; }

        public DateTimeOffset? ValidUntil { get; set; }
    }
}
