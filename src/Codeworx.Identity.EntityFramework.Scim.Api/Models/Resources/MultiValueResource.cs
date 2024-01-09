using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources
{
    public abstract class MultiValueResource
    {
        public virtual string? Type { get; set; }

        public virtual bool Primary { get; set; }

        public virtual string? Display { get; set; }

        [JsonPropertyName("$ref")]
        public virtual string? Ref { get; set; }
    }
}