using System.Collections.Generic;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models
{
    [JsonConverter(typeof(RequestResourceConverter<GroupResource>))]
    public class GroupRequest : ScimRequest<GroupResource>, IGroupResource, ICommonRequestResource
    {
        public GroupRequest(CommonRequestResource common, GroupResource grp, params ISchemaResource[] extensions)
            : base(common, grp, extensions)
        {
        }

        public string[] Schemas => Common.Schemas;

        public string? ExternalId => Resource.ExternalId;

        public string? DisplayName => Resource.DisplayName;

        public List<GroupMemberResource>? Members => Resource.Members;
    }
}
