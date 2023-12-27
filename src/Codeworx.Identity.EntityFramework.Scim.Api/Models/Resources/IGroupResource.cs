using System.Collections.Generic;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public interface IGroupResource
    {
        string? DisplayName { get; }

        string? ExternalId { get; }

        List<GroupMemberResource>? Members { get; }
    }
}