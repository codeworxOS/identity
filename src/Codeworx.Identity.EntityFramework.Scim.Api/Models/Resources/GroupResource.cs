using System.Collections.Generic;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class GroupResource : IGroupResource, ISchemaResource, IResourceType
    {
        public string? DisplayName { get; set; }

        public string? ExternalId { get; set; }

        public List<GroupMemberResource>? Members { get; set; }

        string IResourceType.ResourceType => ScimConstants.ResourceTypes.User;

        string ISchemaResource.Schema => ScimConstants.Schemas.User;
    }
}
