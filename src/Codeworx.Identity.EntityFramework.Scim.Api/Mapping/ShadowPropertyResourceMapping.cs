using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ShadowPropertyResourceMapping<TEntity, TResource, TData> : ResourceMapping<TEntity, TResource, TData>, IPropertyName
        where TEntity : class
        where TResource : IScimResource
    {
        public ShadowPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, string propertyName)
            : base(p => EF.Property<TData>(p.Entity, propertyName), resourceExpression)
        {
            PropertyName = propertyName;
        }

        public string PropertyName { get; }

        public override Task CopyValueAsync(DbContext db, TEntity entity, TResource resource, Guid providerId)
        {
            var entry = db.Entry(entity);
            entry.Property(PropertyName).CurrentValue = GetResourceValue(resource);

            return Task.CompletedTask;
        }

        public override Expression<Func<ScimEntity<TEntity>, bool>>? GetFilter(OperationFilterNode operationFilterNode)
        {
            var path = operationFilterNode.Path.Members;

            if (path.Equals(ResourcePath, StringComparison.OrdinalIgnoreCase))
            {
                var value = Expression.Constant(operationFilterNode.Value);
                var body = Expression.Equal(Entity.Body, value);

                return Expression.Lambda<Func<ScimEntity<TEntity>, bool>>(body, Entity.Parameters[0]);
            }

            return null;
        }

        protected override IEnumerable<MappedPropertyInfo> GetMappedProperties(DbContext db)
        {
            if (Resource.Body is MemberExpression member)
            {
                var column = db.Model.FindEntityType(typeof(TEntity))?.FindProperty(PropertyName);
                if (column != null)
                {
                    yield return new MappedPropertyInfo(member.Member, column, member.Expression);
                }
            }
        }
    }
}
