using System.Linq;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    public abstract class ScimResponse<T>
        where T : ISchemaResource, IResourceType
    {
        public ScimResponse(ScimResponseInfo info, T resource, ISchemaResource[] extensions)
        {
            Common = new CommonResponseResource(
                info.Id,
                new ScimMetadata(resource.ResourceType, info.Location, info.Created, info.LastModified))
            {
                ExternalId = info.ExternalId,
                Schemas = new[] { resource.Schema }.Concat(extensions.Select(p => p.Schema)).ToArray(),
            };
            Resource = resource;
            Extensions = extensions;
        }

        [JsonIgnore]
        public CommonResponseResource Common { get; }

        [JsonIgnore]
        public T Resource { get; }

        [JsonIgnore]
        public ISchemaResource[] Extensions { get; }
    }
}