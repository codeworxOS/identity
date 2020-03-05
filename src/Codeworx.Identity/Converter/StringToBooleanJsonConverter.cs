using System;
using Newtonsoft.Json;

namespace Codeworx.Identity.Converter
{
    public class StringToBooleanJsonConverter : JsonConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            switch (reader.Value.ToString().ToLower().Trim())
            {
                case "true":
                case "yes":
                case "y":
                case "1":
                case "on":
                    return true;

                case "false":
                case "no":
                case "n":
                case "0":
                    return false;
            }

            return new JsonSerializer().Deserialize(reader, objectType);
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(bool);
        }
    }
}