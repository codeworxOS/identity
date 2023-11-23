using System;
using System.Linq.Expressions;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions
{
    public interface ISchemaParameterDescription<TEntity>
    {
        string Schema { get; set; }

        Expression<Func<TEntity, object>> Property { get; set; }

        string Display { get; set; }

        bool Unique { get; set; }
    }
}
