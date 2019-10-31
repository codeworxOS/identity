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

        public DbSet<ClientConfiguration> ClientConfigurations { get; set; }

        public DbSet<ExternalAuthenticationProvider> ExternalAuthenticationProviders { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfiguration(new RightHolderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TenantEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new UserRoleEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new TenantUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ClientConfigurationEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ExternalAuthenticationProviderEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new AuthenticationProviderUserEntityTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ProviderFilterEntityTypeConfiguration());

            modelBuilder.Ignore<ClientScope>();
            modelBuilder.Ignore<ScopeClaim>();
        }
    }
}