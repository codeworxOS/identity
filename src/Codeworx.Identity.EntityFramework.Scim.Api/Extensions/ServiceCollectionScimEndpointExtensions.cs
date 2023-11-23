using System;
using System.Linq.Expressions;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionScimEndpointExtensions
    {
        public static IServiceCollection AddScimEndpoint<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IContextWrapper, DbContextWrapper<TContext>>();

            services.AddScimSchemaAttribute<User>(SchemaConstants.User, d => d.Name, display: "userName", unique: true);
            services.AddScimSchemaAttribute<Group>(SchemaConstants.Group, d => d.Name, display: "displayName", unique: true);

            return services;
        }

        public static IServiceCollection AddScimSchemaAttribute<TEntity>(this IServiceCollection services, string schema, Expression<Func<TEntity, object>> property, string display = null, bool unique = false)
        {
            services.AddSingleton<ISchemaParameterDescription<TEntity>, SchemaParameterDescription<TEntity>>(sp => new SchemaParameterDescription<TEntity>(schema, property, display, unique));
            return services;
        }
    }
}
