using System;
using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.EntityFrameworkCore.Api.Model;
using Codeworx.Identity.EntityFrameworkCore.Api.NSwag;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;
using NSwag.Generation.AspNetCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityNSwagServiceCollectionExtensions
    {
        public static IServiceCollection AddOpenApiDocument<TContext>(this IServiceCollection services, Action<AspNetCoreOpenApiDocumentGeneratorSettings, IServiceProvider> configuration = null)
            where TContext : DbContext
        {
            return services
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<UserData, User>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<UserListData, User>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<UserInsertData, User>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<UserUpdateData, User>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<GroupListData, Group>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<GroupInsertData, Group>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<GroupData, Group>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<TenantListData, Tenant>>()
                    .AddSingleton<IAdditionalDataEntityMapping, AdditionalDataEntityMapping<TenantInsertData, Tenant>>()
                    .AddOpenApiDocument((settings, serviceProvider) =>
                    {
                        settings.SchemaProcessors.Add(new DbContextSchemaProcessor<TContext>(serviceProvider));
                        configuration?.Invoke(settings, serviceProvider);
                    });
        }
    }
}
