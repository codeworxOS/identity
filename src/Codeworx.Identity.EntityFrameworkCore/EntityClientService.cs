using System;
using System.Collections.Immutable;
using System.Linq;
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

        public virtual async Task<IClientRegistration> GetById(string clientIdentifier)
        {
            var id = Guid.Parse(clientIdentifier);

            var result = await _context.Set<ClientConfiguration>()
                .Include(p => p.ValidRedirectUrls)
                .Include(p => p.User)
                .SingleOrDefaultAsync(p => p.Id == id);

            if (result == null)
            {
                return null;
            }

            return new Data.ClientRegistration
            {
                ClientId = result.Id.ToString("N"),
                ClientSecretHash = result.ClientSecretHash,
                ClientType = result.ClientType,
                TokenExpiration = result.TokenExpiration,
                ValidRedirectUrls = result.ValidRedirectUrls.Select(p => new Uri(p.Url)).ToImmutableList(),
            };
        }
    }
}