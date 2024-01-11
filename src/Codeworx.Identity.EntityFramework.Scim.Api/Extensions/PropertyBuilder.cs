using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;

namespace Microsoft.Extensions.DependencyInjection
{
    public class PropertyBuilder<TResource, TEntity>
        where TResource : IScimResource
        where TEntity : class
    {
        private readonly IServiceCollection _services;

        public PropertyBuilder(IServiceCollection services)
        {
            _services = services;
        }

        public PropertyBuilder<TResource, TEntity> AddMapping(IResourceMapping<TEntity> mapping)
        {
            _services.AddSingleton(mapping);

            return this;
        }

        public PropertyBuilder<TResource, TEntity> AddClrProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<ScimEntity<TEntity>, TData>> entityExpression, bool readOnly = false)
        {
            _services.AddSingleton<IResourceMapping<TEntity>>(new ClrPropertyResourceMapping<TEntity, TResource, TData>(resourceExpression, entityExpression, readOnly));

            return this;
        }

        public PropertyBuilder<TResource, TEntity> AddClrProperty<TData>(
            Expression<Func<TResource, TData>> resourceExpression,
            Expression<Func<ScimEntity<TEntity>, TData>> entityExpression,
            Action<TEntity, TData?> setValueDelegate)
        {
            _services.AddSingleton<IResourceMapping<TEntity>>(new ClrPropertyResourceMapping<TEntity, TResource, TData>(resourceExpression, entityExpression, setValueDelegate));

            return this;
        }

        public PropertyBuilder<TResource, TEntity> AddNavigationProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<ScimEntity<TEntity>, TData>> entityExpression)
            where TData : class, IEnumerable<MultiValueResource>
        {
            _services.AddSingleton<IResourceMapping<TEntity>>(new MultiValueNavigationPropertyResourceMapping<TEntity, TResource, TData>(resourceExpression, entityExpression, true));

            return this;
        }

        public PropertyBuilder<TResource, TEntity> AddShadowProperty<TMultiValueResource, TData>(Expression<Func<TResource, ICollection<TMultiValueResource>>> resourceExpression, string propertyName, string type, bool primary = false)
            where TMultiValueResource : MultiValueResource<TData>, new()
        {
            _services.AddSingleton<IResourceMapping<TEntity>>(new MultiValueShadowPropertyResourceMapping<TEntity, TResource, TMultiValueResource, TData>(resourceExpression, propertyName, type, primary));

            return this;
        }

        public PropertyBuilder<TResource, TEntity> AddShadowProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, string propertyName)
        {
            _services.AddSingleton<IResourceMapping<TEntity>>(new ShadowPropertyResourceMapping<TEntity, TResource, TData>(resourceExpression, propertyName));

            return this;
        }
    }
}
