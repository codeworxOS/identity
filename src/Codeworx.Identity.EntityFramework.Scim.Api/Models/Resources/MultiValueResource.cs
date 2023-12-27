using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public class MultiValueResource<TValue>
    {
        public TValue Value { get; set; } = default!;

        public string? Type { get; set; }

        public bool Primary { get; set; }

        public string? Display { get; set; }

        [JsonPropertyName("$ref")]
        public string? Ref { get; set; }
    }
}