using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class MultiValueNavigationPropertyResourceMapping<TEntity, TResource, TData> : ResourceMapping<TEntity, TResource, TData>, IReadOnly
        where TEntity : class
        where TResource : IScimResource
        where TData : IEnumerable<MultiValueResource>
    {
        public MultiValueNavigationPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<TEntity, TData>> entityExpression, bool readOnly = false)
            : base(entityExpression, resourceExpression)
        {
            ReadOnly = readOnly;
        }

        public bool ReadOnly { get; }

        public override Task CopyValueAsync(DbContext db, TEntity entity, TResource resource)
        {
            var value = GetResourceValue(resource);
            ////_setValueDelegate(entity, value);

            return Task.CompletedTask;
        }
    }
}
