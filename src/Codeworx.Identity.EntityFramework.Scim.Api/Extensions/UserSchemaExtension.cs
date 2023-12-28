using System;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public class UserSchemaExtension : IUserSchemaExtension
    {
        public UserSchemaExtension(string schema, Type type)
        {
            Schema = schema;
            TargetType = type;
        }

        public string Schema { get; }

        public Type TargetType { get; }
    }
}
