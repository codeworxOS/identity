using System;
using System.Linq;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityUserProvider<TContext> : IUserProvider
        where TContext : DbContext
    {
        private readonly TContext _context;

        public EntityUserProvider(TContext context)
        {
            _context = context;
        }

        public virtual async Task<IUser> GetUserByIdentifierAsync(string identity)
        {
            var userSet = _context.Set<User>();
            var id = Guid.Parse(identity);

            var user = await userSet.Where(p => p.Id == id).SingleOrDefaultAsync();

            return user;
        }

        public virtual async Task<IUser> GetUserByNameAsync(string username)
        {
            var userSet = _context.Set<User>();

            var user = await userSet.Where(p => p.Name == username).SingleOrDefaultAsync();

            return user;
        }
    }
}