using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping
{
    public abstract class ResourceMapping<TEntity, TResource, TData> : IResourceMapping<TEntity, TResource, TData>
        where TEntity : class
        where TResource : IScimResource
    {
        private readonly Expression<Func<TEntity, TData>> _entityExpression;
        private readonly Expression<Func<TResource, TData>> _resourceExpression;
        private readonly Func<TResource, TData> _resourceValueDelegate;

        public ResourceMapping(
            Expression<Func<TEntity, TData>> entityExpression,
            Expression<Func<TResource, TData>> resourceExpression)
        {
            _entityExpression = entityExpression;
            _resourceExpression = resourceExpression;
            _resourceValueDelegate = _resourceExpression.Compile();
        }

        public Expression<Func<TResource, TData>> Resource => _resourceExpression;

        public Expression<Func<TEntity, TData>> Entity => _entityExpression;

        public LambdaExpression ResourceExpression => Resource;

        public LambdaExpression EntityExpression => Entity;

        public TData GetResourceValue(TResource resource)
        {
            return _resourceValueDelegate(resource);
        }

        public virtual async Task ToDatabaseAsync(DbContext db, TEntity entity, ISchemaResource resource)
        {
            if (resource is TResource typed)
            {
                await CopyValueAsync(db, entity, typed);
            }
        }

        public abstract Task CopyValueAsync(DbContext db, TEntity entity, TResource resource);

        public virtual async IAsyncEnumerable<SchemaInfo> GetSchemaAttributesAsync(DbContext db)
        {
            await Task.Yield();

            foreach (var property in GetMappedProperties(db))
            {
                var paths = new List<SchemaPath>();

                var parent = property.Parent;
                var name = property.Member.GetJsonName();

                paths.Insert(0, new SchemaPath(name, false));

                while (parent is MemberExpression memberExpression)
                {
                    var parentName = memberExpression.Member.GetJsonName();
                    paths.Insert(0, new SchemaPath(parentName, memberExpression.Type.IsEnumerable()));
                    parent = memberExpression.Expression;
                }

                if (parent != Resource.Parameters[0])
                {
                    throw new NotSupportedException("The resource expression does not contain a propper path to the root parameter! e.g. p => value.FirstName instead of p => p.FirstName");
                }

                yield return new SchemaInfo(paths, "string", typeof(TResource));
            }
        }

        protected abstract IEnumerable<MappedPropertyInfo> GetMappedProperties(DbContext db);

        protected class MappedPropertyInfo
        {
            public MappedPropertyInfo(MemberInfo member, IProperty? column, Expression? parent)
            {
                Member = member;
                Column = column;
                Parent = parent;
            }

            public MemberInfo Member { get; }

            public Expression? Parent { get; }

            public IProperty? Column { get; }
        }
    }
}
