﻿using System;
using Codeworx.Identity.Configuration;
using Codeworx.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityEntityFrameworkCoreSqlServerIdentityServiceBuilderExtensions
    {
        [Obsolete("Use UseDbContestSqlServer method instead",true)]
        public static IIdentityServiceBuilder UseDbContextSqlite(this IIdentityServiceBuilder builder, Func<IServiceProvider, string> connectionStringFactory)
        {
            throw new NotSupportedException();
        }

        [Obsolete("Use UseDbContestSqlServer method instead", true)]
        public static IIdentityServiceBuilder UseDbContextSqlite(this IIdentityServiceBuilder builder, string connectionString)
        {
            throw new NotSupportedException();
        }

        public static IIdentityServiceBuilder UseDbContextSqlServer(this IIdentityServiceBuilder builder, Func<IServiceProvider, string> connectionStringFactory)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(
                (sp, builder) => builder.UseSqlServer(connectionStringFactory(sp), x => x.MigrationsAssembly(GetMigrationAssemblyName()))
                                        .UseDataMigrations());

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        public static IIdentityServiceBuilder UseDbContextSqlServer(this IIdentityServiceBuilder builder, string connectionString)
        {
            builder.ServiceCollection.AddDbContext<CodeworxIdentityDbContext>(
                (sp, builder) => builder.UseSqlServer(connectionString, x => x.MigrationsAssembly(GetMigrationAssemblyName()))
                                        .UseDataMigrations());

            return builder.UseDbContext<CodeworxIdentityDbContext>();
        }

        private static string GetMigrationAssemblyName()
        {
            return typeof(CodeworxIdentityEntityFrameworkCoreSqlServerIdentityServiceBuilderExtensions).Assembly.GetName().Name;
        }
    }
}