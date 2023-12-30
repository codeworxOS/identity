using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

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
    }
}
