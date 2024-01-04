using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ShadowPropertyResourceMapping<TEntity, TResource, TData> : ResourceMapping<TEntity, TResource, TData>, IPropertyName
        where TEntity : class
        where TResource : IScimResource
    {
        public ShadowPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, string propertyName)
            : base(p => EF.Property<TData>(p, propertyName), resourceExpression)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public override Task CopyValueAsync(DbContext db, TEntity entity, TResource resource)
        {
            var entry = db.Entry(entity);
            entry.Property(PropertyName).CurrentValue = GetResourceValue(resource);

            return Task.CompletedTask;
        }
    }
}
