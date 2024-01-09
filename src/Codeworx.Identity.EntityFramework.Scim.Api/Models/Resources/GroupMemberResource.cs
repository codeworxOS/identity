using Codeworx.Identity.Configuration;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class GroupMemberResource : MultiValueResource<string>
    {
        [ScimCanonicalValues("User", "Group")]
        public override string? Type { get; set; }

        [ScimReferenceTypes("User", "Group")]
        public override string? Ref { get; set; }
    }
}