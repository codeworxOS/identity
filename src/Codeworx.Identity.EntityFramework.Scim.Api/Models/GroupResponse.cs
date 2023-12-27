using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    [JsonConverter(typeof(ResponseResourceConverter<GroupResource>))]
    public class GroupResponse : ScimResponse<GroupResource>, IGroupResource, ICommonResponseResource
    {
        public GroupResponse(ScimResponseInfo info, GroupResource grp, params ISchemaResource[] extensions)
            : base(info, grp, extensions)
        {
        }

        public string[] Schemas => Common.Schemas;

        public string? ExternalId => Resource.ExternalId;

        public string? DisplayName => Resource.DisplayName;

        public List<GroupMemberResource>? Members => Resource.Members;

        public ScimMetadata Meta => Common.Meta;

        public string Id => Common.Id;
    }
}
