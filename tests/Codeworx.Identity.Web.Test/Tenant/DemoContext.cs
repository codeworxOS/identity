using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.Web.Test.Tenant
{
    public class DemoContext : DbContext
    {
        public DemoContext(DbContextOptions<DemoContext> options)
            : base(options)
        {
        }

        public DbSet<Order> Orders { get; set; }

    }
}
