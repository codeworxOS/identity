using System.Collections.Generic;
using System.Text.Json;
using System.Text.Json.Serialization;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Microsoft.AspNetCore.Mvc;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Serialization
{
    public class SerializationSetup : ISerializationSetup
    {
        private static readonly JsonSerializerOptions _serializeOptions;
        private readonly JsonSerializerOptions _deserializeOptions;
        private readonly JsonOptions _jsonOptions;

        static SerializationSetup()
        {
            _serializeOptions = new JsonSerializerOptions()
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            };
        }

        public SerializationSetup(IEnumerable<ISchemaExtension> schemaExtensions)
        {
            _deserializeOptions = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull,
            };

            _jsonOptions = new JsonOptions();
            _jsonOptions.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
            _jsonOptions.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
            _jsonOptions.JsonSerializerOptions.DefaultIgnoreCondition = System.Text.Json.Serialization.JsonIgnoreCondition.WhenWritingNull;

            foreach (var extension in schemaExtensions)
            {
                _deserializeOptions.Converters.Add(new ScimSchemaConverter(extension.Schema, extension.TargetType));
                _jsonOptions.JsonSerializerOptions.Converters.Add(new ScimSchemaConverter(extension.Schema, extension.TargetType));
            }
        }

        public static JsonSerializerOptions CreateOptionsForSerialize()
        {
            return _serializeOptions;
        }

        public JsonOptions GetJsonFormatterOptions()
        {
            return _jsonOptions;
        }

        public JsonSerializerOptions GetOptionsForDeserialize()
        {
            return _deserializeOptions;
        }

        public JsonSerializerOptions GetOptionsForSerialize()
        {
            return CreateOptionsForSerialize();
        }
    }
}
