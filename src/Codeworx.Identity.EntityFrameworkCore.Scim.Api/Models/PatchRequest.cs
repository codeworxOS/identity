using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models
{
    [JsonConverter(typeof(RequestResourceConverter<PatchResource>))]
    public class PatchRequest : ScimRequest<PatchResource>, IPatchResource, ICommonRequestResource
    {
        public PatchRequest(CommonRequestResource common, PatchResource patch, params ISchemaResource[] extensions)
            : base(common, patch, extensions)
        {
        }

        public List<PatchOperation> Operations => Resource.Operations;

        public string[] Schemas => Common.Schemas;
    }
}
