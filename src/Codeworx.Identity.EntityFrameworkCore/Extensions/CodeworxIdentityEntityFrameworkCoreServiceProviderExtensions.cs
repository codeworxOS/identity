using System;
using System.Threading.Tasks;
using Extensions.EntityFrameworkCore.DataMigration;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CodeworxIdentityEntityFrameworkCoreServiceProviderExtensions
    {
        public static async Task MigrateDatabaseAsync<TContext>(this IServiceProvider serviceProvider)
            where TContext : DbContext
        {
            using (var scope = serviceProvider.CreateScope())
            {
                var context = scope.ServiceProvider.GetService<TContext>();
                await context.Database.MigrateAsync();
                if (((IInfrastructure<IServiceProvider>)context).GetService<DataMigrationOptions>() != null)
                {
                    await context.DataMigrator().MigrateAsync();
                }
            }
        }
    }
}