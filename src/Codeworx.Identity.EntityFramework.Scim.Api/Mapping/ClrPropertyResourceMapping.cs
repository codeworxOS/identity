using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public class ClrPropertyResourceMapping<TEntity, TResource, TData> : ResourceMapping<TEntity, TResource, TData>, IReadOnly
        where TEntity : class
        where TResource : IScimResource
    {
        private readonly Action<TEntity, TData>? _setValueDelegate;

        public ClrPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<TEntity, TData>> entityExpression, bool readOnly = false)
            : base(entityExpression, resourceExpression)
        {
            ReadOnly = readOnly;

            if (!ReadOnly)
            {
                var valueParameter = Expression.Parameter(typeof(TData), "value");

                var body = Expression.Assign(EntityExpression.Body, valueParameter);
                var setValueExpression = Expression.Lambda<Action<TEntity, TData>>(body, entityExpression.Parameters[0], valueParameter);

                _setValueDelegate = setValueExpression.Compile();
            }
        }

        public ClrPropertyResourceMapping(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<TEntity, TData>> entityExpression, Action<TEntity, TData> setValueDelegate)
            : base(entityExpression, resourceExpression)
        {
            ReadOnly = false;
            _setValueDelegate = setValueDelegate;
        }

        public bool ReadOnly { get; }

        public override Task CopyValueAsync(DbContext db, TEntity entity, TResource resource)
        {
            if (_setValueDelegate == null)
            {
                return Task.CompletedTask;
            }

            var value = GetResourceValue(resource);
            _setValueDelegate(entity, value);

            return Task.CompletedTask;
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
