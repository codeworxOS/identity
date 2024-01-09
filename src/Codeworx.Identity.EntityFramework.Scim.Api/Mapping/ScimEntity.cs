namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ScimEntity<TEntity>
    {
        public string ExternalId { get; set; } = null!;

        public TEntity Entity { get; set; } = default!;
    }
}
