﻿using System;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;

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

        public PropertyBuilder<TResource, TEntity> Schema(string schema)
        {
            _services.AddSingleton<ISchemaExtension>(new SchemaExtension(schema, typeof(TResource)));
            return this;
        }

        public PropertyBuilder<TResource, TEntity> AddClrProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, Expression<Func<TEntity, TData>> entityExpression, bool readOnly = false)
        {
            _services.AddSingleton<IResourceMapping<TEntity>>(new ClrPropertyResourceMapping<TEntity, TResource, TData>(resourceExpression, entityExpression, readOnly));

            return this;
        }

        public PropertyBuilder<TResource, TEntity> AddShadowProperty<TData>(Expression<Func<TResource, TData>> resourceExpression, string propertyName)
        {
            _services.AddSingleton<IResourceMapping<TEntity>>(new ShadowPropertyResourceMapping<TEntity, TResource, TData>(resourceExpression, propertyName));

            return this;
        }
    }
}
