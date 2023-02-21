using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Codeworx.Identity.EntityFrameworkCore.MigrationCreator
{
    public class DesignTimeContextFactory : IDesignTimeDbContextFactory<CodeworxIdentityDbContext>
    {
        public CodeworxIdentityDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<CodeworxIdentityDbContext>();

#if MSSQL
            builder.UseSqlServer("Data Source=.;Integrated Security=True; Initial Catalog=IdentityMaster", p => p.MigrationsAssembly("Codeworx.Identity.EntityFrameworkCore.Migrations.SqlServer"))
                .UseDataMigrations();
#endif
#if SQLITE
            builder.UseSqlite("filename=IdentityMaster.sqlite", p => p.MigrationsAssembly("Codeworx.Identity.EntityFrameworkCore.Migrations.Sqlite"))
                            .UseDataMigrations();
#endif

            return new CodeworxIdentityDbContext(builder.Options);
        }
    }
}