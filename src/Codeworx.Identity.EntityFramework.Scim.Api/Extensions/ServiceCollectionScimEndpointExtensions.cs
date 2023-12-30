using System;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionScimEndpointExtensions
    {
        public static IServiceCollection AddScimEndpoint<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IContextWrapper, DbContextWrapper<TContext>>();
            services.AddSingleton(typeof(IResourceMapper<>), typeof(ResourceMapper<>));
            return services;
        }

        public static IServiceCollection AddScimProperties<TEntity, TResource>(this IServiceCollection services, Action<PropertyBuilder<TResource, TEntity>> builderAction)
            where TEntity : class
            where TResource : IScimResource
        {
            var builder = new PropertyBuilder<TResource, TEntity>(services);

            builderAction(builder);

            return services;
        }
    }
}
