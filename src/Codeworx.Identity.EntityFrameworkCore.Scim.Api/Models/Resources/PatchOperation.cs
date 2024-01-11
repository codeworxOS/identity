using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources
{
    public class PatchOperation
    {
        public string? Path { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PatchOp Op { get; set; }

        public object? Value { get; set; }
    }
}