using System;
using System.Linq;
using System.Runtime.Serialization;
using Newtonsoft.Json;

namespace Codeworx.Identity.Converter
{
    public class EnumMemberJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            var member = value.GetType().GetMember(value.ToString());
            var enumMemberAttribute = member.SelectMany(p => p.GetCustomAttributes(typeof(EnumMemberAttribute), false))
                .OfType<EnumMemberAttribute>()
                .FirstOrDefault();

            writer.WriteValue(enumMemberAttribute?.Value ?? value);
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType.IsEnum;
        }
    }
}