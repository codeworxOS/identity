using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.Demo.Database
{
    public class DemoIdentityDbContext : CodeworxIdentityDbContext
    {
        public DemoIdentityDbContext(DbContextOptions<DemoIdentityDbContext> options)
            : base(options)
        {
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(p =>
            {
                p.Property<string>("FirstName").HasMaxLength(400);
                p.Property<string>("LastName").HasMaxLength(400);
                p.Property<string>("Email").HasMaxLength(600).IsRequired();
            });
        }
    }
}
