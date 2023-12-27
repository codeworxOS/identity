using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class GroupMemberResource
    {
        public string? Id { get; set; }

        [JsonPropertyName("$ref")]
        public string? Ref { get; set; }
    }
}