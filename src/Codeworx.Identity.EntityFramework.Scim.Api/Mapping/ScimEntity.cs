using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ScimEntity<TEntity>
    {
        public string ExternalId { get; set; } = null!;

        public Guid ProviderId { get; set; }

        public TEntity Entity { get; set; } = default!;
    }
}
