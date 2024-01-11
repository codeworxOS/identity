namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public interface ISchemaResource : IScimResource
    {
        string Schema { get; }
    }
}
