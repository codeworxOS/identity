namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public interface IResourceTypeResource
    {
        string Name { get; }

        string Endpoint { get; }

        string Schema { get; }

        string? Description { get; }
    }
}