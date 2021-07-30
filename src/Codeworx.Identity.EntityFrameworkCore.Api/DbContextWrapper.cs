using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore.Api
{
    public class DbContextWrapper<TContext> : IContextWrapper
        where TContext : DbContext
    {
        public DbContextWrapper(TContext context)
        {
            Context = context;
        }

        public DbContext Context { get; }
    }
}
