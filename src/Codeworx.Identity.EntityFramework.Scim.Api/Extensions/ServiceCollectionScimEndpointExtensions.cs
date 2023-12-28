using Codeworx.Identity.EntityFrameworkCore.Scim.Api;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
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

            return services;
        }

        public static PropertyBuilder<T> AddScimUserSchemaExtension<T>(this IServiceCollection services, string schema)
            where T : ISchemaResource, new()
        {
            services.AddSingleton<IUserSchemaExtension>(sp => new UserSchemaExtension(schema, typeof(T)));

            return new PropertyBuilder<T>(services);
        }

        public static PropertyBuilder<UserResource> ScimUserPropertyBuilder(this IServiceCollection services)
        {
            return new PropertyBuilder<UserResource>(services);
        }

        public static PropertyBuilder<GroupResource> ScimGroupPropertyBuilder(this IServiceCollection services)
        {
            return new PropertyBuilder<GroupResource>(services);
        }
    }
}
