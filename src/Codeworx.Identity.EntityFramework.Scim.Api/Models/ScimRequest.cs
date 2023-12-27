using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    public abstract class ScimRequest<T>
        where T : ISchemaResource
    {
        public ScimRequest(CommonRequestResource common, T resource, ISchemaResource[] extensions)
        {
            Common = common;
            Resource = resource;
            Extensions = extensions;
        }

        [JsonIgnore]
        public CommonRequestResource Common { get; }

        [JsonIgnore]
        public T Resource { get; }

        [JsonIgnore]
        public ISchemaResource[] Extensions { get; }
    }
}