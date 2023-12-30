using System;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;

namespace Microsoft.Extensions.DependencyInjection
{
    internal class SchemaExtension : ISchemaExtension
    {
        public SchemaExtension(string schema, Type type)
        {
            Schema = schema;
            TargetType = type;
        }

        public string Schema { get; }

        public Type TargetType { get; }
    }
}