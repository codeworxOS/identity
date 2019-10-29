using Codeworx.Identity.EntityFrameworkCore.Mappings;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class CodeworxIdentityDbContext : DbContext
    {
        public CodeworxIdentityDbContext(DbContextOptions<CodeworxIdentityDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }

        public DbSet<Role> Roles { get; set; }

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RightHolderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TenantEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TenantUserEntityTypeConfiguration());
        }
    }
}