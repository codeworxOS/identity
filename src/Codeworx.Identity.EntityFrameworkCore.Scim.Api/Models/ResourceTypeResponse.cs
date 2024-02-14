using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models
{
    [JsonConverter(typeof(ResponseResourceConverter<ResourceTypeResource>))]
    public class ResourceTypeResponse : ScimResponse<ResourceTypeResource>, IResourceTypeResource, ICommonResponseResource
    {
        public ResourceTypeResponse(ScimResponseInfo info, ResourceTypeResource user)
            : base(info, user, new ISchemaResource[] { })
        {
        }

        public ScimMetadata Meta => Common.Meta;

        public string Id => Common.Id;

        public string[] Schemas => Common.Schemas;

        public string Name => Resource.Name;

        public string Endpoint => Resource.Endpoint;

        public string Schema => Resource.Schema;

        public string? Description => Resource.Description;
    }
}