using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Serialization
{
    public class ScimSchemaConverter : JsonConverter<object>
    {
        public ScimSchemaConverter(string schema, Type targetType)
        {
            Schema = schema;
            TargetType = targetType;
        }

        public string Schema { get; set; }

        public Type TargetType { get; set; }

        public override bool CanConvert(Type typeToConvert)
        {
            return false;
        }

        public override object? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }

        public override void Write(Utf8JsonWriter writer, object value, JsonSerializerOptions options)
        {
            throw new NotImplementedException();
        }
    }
}