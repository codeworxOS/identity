using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class MangerResource
    {
        public string? Value { get; set; }

        public string? DisplayName { get; set; }

        [JsonPropertyName("$ref")]
        public string? Ref { get; set; }
    }
}