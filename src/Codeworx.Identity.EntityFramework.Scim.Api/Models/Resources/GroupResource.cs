using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class GroupResource : IGroupResource, ISchemaResource, IResourceType
    {
        public string? DisplayName { get; set; }

        public string? ExternalId { get; set; }

        public List<GroupMemberResource>? Members { get; set; }

        string IResourceType.ResourceType => ScimConstants.ResourceTypes.Group;

        string ISchemaResource.Schema => ScimConstants.Schemas.Group;
    }
}
