using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models
{
    public class RequestResourceConverter<TResource> : JsonConverter<ScimRequest<TResource>>
        where TResource : ISchemaResource
    {
        public override bool CanConvert(Type typeToConvert)
        {
            return typeof(ScimRequest<TResource>).IsAssignableFrom(typeToConvert);
        }

        public override ScimRequest<TResource>? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            var json = JsonNode.Parse(ref reader)!.AsObject();

            var common = json.Deserialize<CommonRequestResource>(options)!;
            var resource = json.Deserialize<TResource>(options)!;

            List<ISchemaResource> extensions = new List<ISchemaResource>();

            foreach (var item in common.Schemas.Where(p => !p.Equals(resource.Schema)))
            {
                if (json.AsObject().TryGetPropertyValue(item, out var node))
                {
                    var converters = options.Converters.OfType<ScimSchemaConverter>();
                    var target = converters.FirstOrDefault(p => p.Schema == item);
                    if (target != null)
                    {
                        if (node.Deserialize(target.TargetType, options) is ISchemaResource schema)
                        {
                            extensions.Add(schema);
                        }
                    }
                }
            }

            return (ScimRequest<TResource>)Activator.CreateInstance(typeToConvert, common, resource, extensions.ToArray())!;
        }

        public override void Write(Utf8JsonWriter writer, ScimRequest<TResource> value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}