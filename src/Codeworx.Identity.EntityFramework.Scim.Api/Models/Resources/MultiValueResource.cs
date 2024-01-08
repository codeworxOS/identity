using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class MultiValueResource
    {
        public string? Type { get; set; }

        public bool Primary { get; set; }

        public string? Display { get; set; }

        [JsonPropertyName("$ref")]
        public string? Ref { get; set; }
    }
}