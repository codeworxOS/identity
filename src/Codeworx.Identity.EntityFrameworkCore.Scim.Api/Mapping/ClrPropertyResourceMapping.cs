using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ClrPropertyResourceMapping<TEntity, TResource, TData> : ResourceMapping<TEntity, TResource, TData>, IReadOnly
        where TEntity : class
        where TResource : IScimResource
    {
        private readonly Action<TEntity, TData?>? _setValueDelegate;

        public ClrPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<ScimEntity<TEntity>, TData>> entityExpression, bool readOnly = false)
            : base(entityExpression, resourceExpression)
        {
            ReadOnly = readOnly;

            if (!ReadOnly)
            {
                var valueParameter = Expression.Parameter(typeof(TData?), "value");
                var entityParameter = Expression.Parameter(typeof(TEntity), "entity");
                var scimEntityParameter = entityExpression.Parameters[0];

                var body = EntityExpression.Body.Replace<MemberExpression>(
                    p => p.Expression == scimEntityParameter && p.Member.Name == nameof(ScimEntity<object>.Entity),
                    entityParameter);

                body = Expression.Assign(body, valueParameter);
                var setValueExpression = Expression.Lambda<Action<TEntity, TData?>>(body, entityParameter, valueParameter);

                _setValueDelegate = setValueExpression.Compile();
            }
        }

        public ClrPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<ScimEntity<TEntity>, TData>> entityExpression, Action<TEntity, TData?> setValueDelegate)
            : base(entityExpression, resourceExpression)
        {
            ReadOnly = false;
            _setValueDelegate = setValueDelegate;
        }

        public bool ReadOnly { get; }

        public override Task CopyValueAsync(DbContext db, TEntity entity, TResource resource, Guid providerId)
        {
            if (_setValueDelegate == null)
            {
                return Task.CompletedTask;
            }

            var value = GetResourceValue(resource);

            ////if (value == null && Required)
            ////{

            ////}

            _setValueDelegate(entity, value);

            return Task.CompletedTask;
        }

        public override Expression<Func<ScimEntity<TEntity>, bool>>? GetFilter(OperationFilterNode operationFilterNode)
        {
            if (operationFilterNode.Path.Members.Equals(ResourcePath, StringComparison.OrdinalIgnoreCase))
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
                IProperty? column = null;

                if (EntityExpression.Body is MemberExpression entityMember)
                {
                    column = db.Model.FindEntityType(entityMember.Expression!.Type)?.FindProperty(entityMember.Member);
                }

                yield return new MappedPropertyInfo(member.Member, column, member.Expression);
            }
        }
    }
}
