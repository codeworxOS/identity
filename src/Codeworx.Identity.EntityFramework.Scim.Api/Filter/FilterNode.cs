using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter
{
    public abstract class FilterNode
    {
        public Expression<Func<ScimEntity<TEntity>, bool>> ToExpression<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
            where TEntity : class
        {
            return Convert<TEntity>(mappings);
        }

        public abstract Expression<Func<ScimEntity<TEntity>, bool>> Convert<TEntity>(IEnumerable<IResourceMapping<TEntity>> mappings)
            where TEntity : class;
    }
}
