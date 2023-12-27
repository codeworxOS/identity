using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Models
{
    public class ResponseResourceConverter<TResource> : JsonConverter<ScimResponse<TResource>>
        where TResource : ISchemaResource, IResourceType
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ScimResponse<TResource>).IsAssignableFrom(typeToConvert);
        }

        public override ScimResponse<TResource>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, ScimResponse<TResource> value, JsonSerializerOptions options)
        {
            options = new JsonSerializerOptions(options)
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingDefault,
            };

            var result = JsonSerializer.SerializeToNode(value.Resource, options)!.AsObject();
            var common = JsonSerializer.SerializeToNode(value.Common, options)!.AsObject();

            foreach (var item in common.ToList())
            {
                common.Remove(item.Key);

                result.Add(item.Key, item.Value);
            }

            foreach (var item in value.Extensions)
            {
                result.Add(item.Schema, JsonSerializer.SerializeToNode(item, item.GetType(), options));
            }

            result.WriteTo(writer, options);
        }
    }
}