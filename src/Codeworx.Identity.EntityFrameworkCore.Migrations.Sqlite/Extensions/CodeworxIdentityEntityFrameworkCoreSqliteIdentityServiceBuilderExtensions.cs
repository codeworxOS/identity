using System;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityEntityFrameworkCoreSqliteIdentityServiceBuilderExtensions
    {
        public static IIdentityServiceBuilder UseDbContextSqlite(this IIdentityServiceBuilder builder, Func<IServiceProvider, string> connectionStringFactory)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(
                (sp, builder) => builder.UseSqlite(connectionStringFactory(sp), x => x.MigrationsAssembly(GetMigrationAssemblyName()))
                                        .UseDataMigrations());

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        public static IIdentityServiceBuilder UseDbContextSqlite(this IIdentityServiceBuilder builder, string connectionString)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(
                (sp, builder) => builder.UseSqlite(connectionString, x => x.MigrationsAssembly(GetMigrationAssemblyName()))
                                        .UseDataMigrations());

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        private static string GetMigrationAssemblyName()
        {
            return typeof(CodeworxIdentityEntityFrameworkCoreSqliteIdentityServiceBuilderExtensions).Assembly.GetName().Name;
        }
    }
}