using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class ResourceTypeResource : IResourceTypeResource, ISchemaResource, IResourceType
    {
        public ResourceTypeResource(string endpoint, string name, string schema)
        {
            Endpoint = endpoint;
            Name = name;
            Schema = schema;
        }

        public string Endpoint { get; set; }

        public string Name { get; set; }

        public string Schema { get; set; }

        public string? Description { get; set; }

        string ISchemaResource.Schema => ScimConstants.Schemas.ResourceType;

        string IResourceType.ResourceType => ScimConstants.ResourceTypes.ResourceType;
    }
}
