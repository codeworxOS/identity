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

        public DbSet<Tenant> Tenants { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Ignore<RightHolder>();

            modelBuilder.Entity<User>()
                        .ToTable("User");

            modelBuilder.Entity<Tenant>()
                        .ToTable("Tenant");

            modelBuilder.Entity<UserRole>()
                        .HasKey(p => new { p.UserId, p.RoleId });

            modelBuilder.Entity<TenantUser>()
                        .HasKey(p => new { p.RightHolderId, p.TenantId });
        }
    }
}