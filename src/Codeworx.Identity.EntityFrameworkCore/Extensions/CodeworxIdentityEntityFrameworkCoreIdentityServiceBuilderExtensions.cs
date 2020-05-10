using System;
using System.Linq;
using Codeworx.Identity.Cache;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.Cryptography;
using Codeworx.Identity.EntityFrameworkCore.Cache;
using Codeworx.Identity.EntityFrameworkCore.ExternalLogin;
using Codeworx.Identity.ExternalLogin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public static class CodeworxIdentityEntityFrameworkCoreIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder UseDbContext(this IIdentityServiceBuilder builder, Action<IServiceProvider, DbContextOptionsBuilder> contextBuilder)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(contextBuilder);

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        public static IIdentityServiceBuilder UseDbContext(this IIdentityServiceBuilder builder, Action<DbContextOptionsBuilder> contextBuilder)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(contextBuilder);

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        public static IIdentityServiceBuilder UseDbContext<TContext>(this IIdentityServiceBuilder builder)
            where TContext : DbContext
        {
            var result = builder
                         .UserProvider<EntityUserService<TContext>>()
                         .Provider<EntityExternalLoginProvider<TContext>>()
                         .ReplaceService<ITenantService, EntityTenantService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IDefaultTenantService, EntityUserService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IClientService, EntityClientService<TContext>>(ServiceLifetime.Scoped)
                         .ReplaceService<IAuthorizationCodeCache, AuthorizationCodeCache<TContext>>(ServiceLifetime.Scoped);

            result.ServiceCollection.AddSingleton<IProcessorTypeLookup, WindowsLoginProcessorLookup>();
            result.ServiceCollection.AddSingleton<IProcessorTypeLookup, ExternalOAuthLoginProcessorLookup>();

            if (result.ServiceCollection.All(p => p.ServiceType != typeof(Pbkdf2Options)))
            {
                result = result.Pbkdf2();
            }

            return result;
        }
    }
}