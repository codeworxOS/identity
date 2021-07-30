using Codeworx.Identity.EntityFrameworkCore;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.Api.Test
{
    internal class TestIdentityContext : CodeworxIdentityDbContext
    {
        public TestIdentityContext(DbContextOptions<TestIdentityContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<User>(p =>
            {
                p.Property<string>("FirstName").HasMaxLength(200);
                p.Property<string>("LastName").HasMaxLength(200);
                p.Property<string>("Email").HasMaxLength(500).IsRequired();
            });
        }
    }
}