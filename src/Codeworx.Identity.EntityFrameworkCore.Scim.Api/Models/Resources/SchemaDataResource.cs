using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class SchemaDataResource : ISchemaDataResource, ISchemaResource, IResourceType
    {
        public SchemaDataResource(string id, string name, List<SchemaDataAttributeResource> attributes)
        {
            Id = id;
            Name = name;
            Attributes = attributes;
        }

        [JsonIgnore]
        public string Id { get; }

        public string Name { get; set; }

        public string? Description { get; set; }

        public List<SchemaDataAttributeResource> Attributes { get; set; }

        string ISchemaResource.Schema => ScimConstants.Schemas.Schema;

        string IResourceType.ResourceType => ScimConstants.ResourceTypes.Schema;
    }
}
