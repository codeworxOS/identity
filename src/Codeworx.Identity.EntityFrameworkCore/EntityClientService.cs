using System;
using System.Threading.Tasks;
using Codeworx.Identity.EntityFrameworkCore.Model;
using Codeworx.Identity.Model;
using Microsoft.EntityFrameworkCore;

namespace Codeworx.Identity.EntityFrameworkCore
{
    public class EntityClientService<TContext> : IClientService
        where TContext : DbContext
    {
        private readonly TContext _context;

        public EntityClientService(TContext context)
        {
            _context = context;
        }

        public async Task<IClientRegistration> GetById(string clientIdentifier)
        {
            var id = Guid.Parse(clientIdentifier);

            return await _context.Set<ClientConfiguration>()
                .Include(p => p.ValidRedirectUrls)
                .SingleOrDefaultAsync(p => p.Id == id);
        }
    }
}