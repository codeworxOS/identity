﻿using System;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Extensions;
using Codeworx.Identity.EntityFrameworkCore.Scim.Api.Mapping;
using Codeworx.Identity.EntityFrameworkCore.Scim.Models;
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
            services.AddSingleton<IResourceMapping<User>>(new ClrPropertyResourceMapping<User, UserResource, string>(p => p.UserName!, p => p.Name));
            services.AddSingleton<IResourceMapping<Group>>(new ClrPropertyResourceMapping<Group, GroupResource, string>(p => p.DisplayName!, p => p.Name));
            services.AddSingleton<IResourceMapping<User>>(new ClrPropertyResourceMapping<User, ScimResponseInfo, string>(p => p.Id, p => p.Id.ToString("N"), true));
            services.AddSingleton<IResourceMapping<User>>(new ClrPropertyResourceMapping<User, ScimResponseInfo, DateTime?>(p => p.Created, p => p.Created, true));
            services.AddSingleton<IResourceMapping<User>>(new ClrPropertyResourceMapping<User, ScimResponseInfo, DateTime?>(p => p.LastModified, p => p.Created, true));

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
