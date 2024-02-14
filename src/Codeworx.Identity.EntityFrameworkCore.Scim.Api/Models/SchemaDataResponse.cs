using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models
{
    [JsonConverter(typeof(ResponseResourceConverter<SchemaDataResource>))]
    public class SchemaDataResponse : ScimResponse<SchemaDataResource>, ISchemaDataResource, ICommonResponseResource
    {
        public SchemaDataResponse(ScimResponseInfo info, SchemaDataResource resource)
            : base(info, resource, new ISchemaResource[] { })
        {
        }

        public ScimMetadata Meta => Common.Meta;

        public string Id => Common.Id;

        public string[] Schemas => Common.Schemas;

        public string Name => Resource.Name;

        public string? Description => Resource.Description;

        public List<SchemaDataAttributeResource> Attributes => Resource.Attributes;
    }
}
