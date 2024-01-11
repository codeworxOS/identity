using System;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Filter;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Models.Resources;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class ServiceCollectionScimEndpointExtensions
    {
        public static IServiceCollection AddScimEndpoint<TContext>(this IServiceCollection services)
            where TContext : DbContext
        {
            services.AddScoped<IContextWrapper, DbContextWrapper<TContext>>();
            services.AddSingleton<IFilterParser, FilterParser>();
            services.AddSingleton(typeof(IResourceMapper<>), typeof(ResourceMapper<>));

            services.AddScimSchema<UserResource>(ScimConstants.ResourceTypes.User, ScimConstants.Schemas.User);
            services.AddScimSchema<GroupResource>(ScimConstants.ResourceTypes.Group, ScimConstants.Schemas.Group);
            services.AddScimSchema<EnterpriseUserResource>(ScimConstants.ResourceTypes.EnterpriseUser, ScimConstants.Schemas.EnterpriseUser);

            services.AddScimProperties<User, UserResource>(d => d
                                                        .AddExternalIdProperty(p => p.ExternalId!)
                                                        .AddClrProperty(d => d.Active, d => !d.Entity.IsDisabled, (d, v) => d.IsDisabled = !v.GetValueOrDefault(true))
                                                        .AddClrProperty(d => d.UserName, d => d.Entity.Name));

            services.AddScimProperties<User, ScimResponseInfo>(d => d.AddClrProperty(d => d.Id, d => d.Entity.Id.ToString("N"), true)
                                                                     .AddClrProperty(d => d.Created, d => d.Entity.Created, true));

            services.AddScimProperties<Group, GroupResource>(d => d
                                                        .AddExternalIdProperty(d => d.ExternalId!)
                                                        .AddClrProperty(d => d.DisplayName, d => d.Entity.Name)
                                                        .AddMembersProperty());

            services.AddScimProperties<Group, ScimResponseInfo>(d => d
                                                        .AddClrProperty(d => d.Id, d => d.Entity.Id.ToString("N"), true));

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

        public static IServiceCollection AddScimSchema<TResource>(this IServiceCollection services, string name, string schema)
        {
            services.AddSingleton<ISchemaExtension>(new SchemaExtension(name, schema, typeof(TResource)));
            return services;
        }
    }
}
