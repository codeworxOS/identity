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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<UserRole>()
                        .HasKey(p => new { p.UserId, p.RoleId });

            modelBuilder.Entity<TenantUser>()
                        .HasKey(p => new { p.RightHolderId, p.TenantId });

            // Remove this when implementing tenant database service
            modelBuilder.Entity<User>()
                        .Ignore(p => p.DefaultTenant)
                        .Ignore(p => p.Tenants);
        }
    }
}