using System.Collections.Generic;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class SchemaDataResource : ISchemaDataResource, ISchemaResource, IResourceType
    {
        public SchemaDataResource(string name, List<SchemaDataAttributeResource> attributes)
        {
            Name = name;
            Attributes = attributes;
        }

        public string Name { get; set; }

        public string? Description { get; set; }

        public List<SchemaDataAttributeResource> Attributes { get; set; }

        string ISchemaResource.Schema => ScimConstants.Schemas.Schema;

        string IResourceType.ResourceType => ScimConstants.ResourceTypes.Schema;
    }
}
