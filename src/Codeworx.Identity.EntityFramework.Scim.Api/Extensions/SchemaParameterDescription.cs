using System;
using System.Linq.Expressions;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public class SchemaParameterDescription<TEntity> : ISchemaParameterDescription<TEntity>
    {
        public SchemaParameterDescription(string schema, Expression<Func<TEntity, object>> property, string display, bool unique)
        {
            Schema = schema;
            Property = property;
            Display = display;
            Unique = unique;
        }

        public string Schema { get; set; }

        public Expression<Func<TEntity, object>> Property { get; set; }

        public string Display { get; set; }

        public bool Unique { get; set; }
    }
}
